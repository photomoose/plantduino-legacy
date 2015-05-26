using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using Rumr.Plantduino.Worker.Handlers;
using Rumr.Plantduino.Worker.Sms;

namespace Rumr.Plantduino.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private TelemetryListener _telemetryListener;
        private ILifetimeScope _scope;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            var tasks = new[]
            {
                _telemetryListener.RunAsync(_cancellationTokenSource.Token)
                //_sensorEventListener.RunAsync(_cancellationTokenSource.Token)
            };

            Task.WhenAll(tasks).Wait();

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
            builder.RegisterType<TemperatureHandler>().As<ITelemetryHandler>();
            builder.RegisterType<LuxHandler>().As<ITelemetryHandler>();
            builder.RegisterType<TelemetryListener>().AsSelf();
            builder.RegisterType<TwilioSmsClient>().As<ISmsClient>();
            var container = builder.Build();

            _scope = container.BeginLifetimeScope();

            _telemetryListener = _scope.Resolve<TelemetryListener>();
            var topicManager = _scope.Resolve<ITopicManager>();

            var tasks = new[]
            {
                _telemetryListener.InitializeAsync(),
                topicManager.CreateTopicAsync(Topics.Commands)
            };

            Task.WhenAll(tasks).Wait();

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
