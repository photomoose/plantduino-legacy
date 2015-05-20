using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

using Newtonsoft.Json;
using Rumr.Plantduino.Worker;

namespace Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private SystemEventListener _systemEventListener;
        private SensorEventListener _sensorEventListener;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            var tasks = new[]
            {
                _systemEventListener.RunAsync(_cancellationTokenSource.Token),
                _sensorEventListener.RunAsync(_cancellationTokenSource.Token)
            };

            Task.WhenAll(tasks).Wait();

            _runCompleteEvent.Set();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            var serviceBusEndpoint = new ServiceBusEndpoint(new Configuration());

            var handlers = new ISystemEventHandler[] {new ColdPeriodBeginHandler(serviceBusEndpoint), new ColdPeriodEndHandler(serviceBusEndpoint)};
            _systemEventListener = new SystemEventListener(new ServiceBusEndpoint(new Configuration()), handlers);
            _sensorEventListener = new SensorEventListener(serviceBusEndpoint, serviceBusEndpoint, new ISensorEventHandler[] {});

            var tasks = new[]
            {
                _systemEventListener.InitializeAsync(),
                _sensorEventListener.InitializeAsync()
            };

            Task.WhenAll(tasks).Wait();

            return base.OnStart();
        }

        public override void OnStop()
        {
            _cancellationTokenSource.Cancel();

            _runCompleteEvent.WaitOne();

            base.OnStop();
        }
    }
}
