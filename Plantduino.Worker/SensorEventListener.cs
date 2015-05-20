using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rumr.Plantduino.Worker
{
    public class SensorEventListener
    {
        private readonly IQueueManager _queueManager;
        private readonly IQueueReceiver _queueReceiver;
        private readonly IEnumerable<ISensorEventHandler> _handlers;

        public SensorEventListener(IQueueManager queueManager, IQueueReceiver queueReceiver, IEnumerable<ISensorEventHandler> handlers)
        {
            _queueManager = queueManager;
            _queueReceiver = queueReceiver;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            await _queueManager.CreateQueueAsync(Queues.SensorEvents);
        }

        public async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var brokeredMessage = await _queueReceiver.ReceiveFromQueueAsync(Queues.SensorEvents);

                if (brokeredMessage != null)
                {
                    Trace.TraceInformation("Received SensorEvent message.");

                    await brokeredMessage.CompleteAsync();
                }
            }
        }
    }
}