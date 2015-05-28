using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using Rumr.Plantduino.Worker.MessageHandlers;
using Rumr.Plantduino.Worker.Sms;
using Rumr.Plantduino.Worker.Subscriptions;
using Rumr.Plantduino.Worker.Telemetry;

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
            builder.RegisterType<Configuration>()
                .As<IServiceBusConfiguration>()
                .As<ITwilioAccount>()
                .As<IConfiguration>();
            builder.RegisterType<ServiceBusEndpoint>()
                .As<ITopicSubscriber>()
                .As<ITopicPublisher>()
                .As<ITopicManager>();
            builder.RegisterType<TemperatureTelemetryHandler>().As<IMessageHandler<TemperatureTelemetry>>();
            builder.RegisterType<LuxTelemetryHandler>().As<IMessageHandler<LuxTelemetry>>();
            builder.RegisterType<TemperatureTelemetrySubscription>().As<ITopicSubscription>();
            builder.RegisterType<LuxTelemetrySubscription>().As<ITopicSubscription>();
            builder.RegisterType<TwilioSmsClient>().As<ISmsClient>();

            var container = builder.Build();

            _scope = container.BeginLifetimeScope();

            var topicManager = _scope.Resolve<ITopicManager>();

            Task.WhenAll(
                topicManager.CreateTopicAsync(TopicNames.Telemetry),
                topicManager.CreateTopicAsync(TopicNames.Commands))
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
