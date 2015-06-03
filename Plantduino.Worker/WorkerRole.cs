using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using Plantduino.Infrastructure.Twilio;
using Rumr.Plantduino.Application;
using Rumr.Plantduino.Application.Services.Handlers.Notifications;
using Rumr.Plantduino.Application.Services.Handlers.Telemetry;
using Rumr.Plantduino.Application.Services.Subscriptions;
using Rumr.Plantduino.Domain;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;
using Rumr.Plantduino.Infrastructure.Elastic;
using Rumr.Plantduino.Infrastructure.ServiceBus;

namespace Rumr.Plantduino.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private ILifetimeScope _scope;
        private IEnumerable<ITopicSubscription> _topicSubscriptions;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            Task.WhenAll(_topicSubscriptions.Select(ts => ts.ListenAsync(_cancellationTokenSource.Token))).Wait();

            _runCompleteEvent.Set();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            var builder = new ContainerBuilder();
            builder.RegisterType<Configuration>().As<IConfiguration>();
            builder.RegisterType<ServiceBusConfiguration>().As<IServiceBusConfiguration>();
            builder.RegisterType<TwilioAccount>().As<ITwilioAccount>();
            builder.RegisterType<ServiceBusEndpoint>()
                .As<ITopicSubscriber>()
                .As<ITopicPublisher>()
                .As<ITopicManager>();
            builder.RegisterType<TemperatureTelemetryHandler>().As<IMessageHandler<TemperatureTelemetry>>();
            builder.RegisterType<LuxTelemetryHandler>().As<IMessageHandler<LuxTelemetry>>();
            builder.RegisterType<ColdSpellEnteredNotificationHandler>().As<IMessageHandler<ColdSpellEnteredNotification>>();
            builder.RegisterType<ColdSpellLeftNotificationHandler>().As<IMessageHandler<ColdSpellLeftNotification>>();
            builder.RegisterType<NotificationSubscription<ColdSpellEnteredNotification>>().As<ITopicSubscription>();
            builder.RegisterType<NotificationSubscription<ColdSpellLeftNotification>>().As<ITopicSubscription>();
            builder.RegisterType<TelemetrySubscription<TemperatureTelemetry>>().As<ITopicSubscription>();
            builder.RegisterType<TelemetrySubscription<LuxTelemetry>>().As<ITopicSubscription>();
#if !DEBUG
            builder.RegisterType<TwilioSmsClient>().As<ISmsClient>();
#else
            builder.RegisterType<TraceSmsClient>().As<ISmsClient>();
#endif
            builder.RegisterType<TelemetryService>().As<ITelemetryService>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<ElasticIndexClient>().As<IIndexService>();

            var container = builder.Build();

            _scope = container.BeginLifetimeScope();

            var topicManager = _scope.Resolve<ITopicManager>();

            Task.WhenAll(
                topicManager.CreateTopicAsync(TopicNames.Telemetry),
                topicManager.CreateTopicAsync(TopicNames.Commands),
                topicManager.CreateTopicAsync(TopicNames.Notifications))
                .Wait();

            _topicSubscriptions = _scope.Resolve<IEnumerable<ITopicSubscription>>();

            Task.WhenAll(_topicSubscriptions.Select(ts => ts.InitializeAsync())).Wait();

            return base.OnStart();
        }

        public override void OnStop()
        {
            _cancellationTokenSource.Cancel();

            _runCompleteEvent.WaitOne();

            _scope.Dispose();

            base.OnStop();
        }
    }
}
