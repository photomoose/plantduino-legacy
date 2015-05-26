using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.Handlers
{
    public class LuxHandler : ITelemetryHandler
    {
        private readonly ITopicSubscriber _topicSubscriber;

        public string SubscriptionName
        {
            get { return Subscriptions.Lux; }
        }

        public Filter SubscriptionFilter
        {
            get { return new SqlFilter(string.Format("MessageType = '{0}'", typeof(LuxTelemetry).Name)); }
        }

        public LuxHandler(ITopicSubscriber topicSubscriber)
        {
            _topicSubscriber = topicSubscriber;
        }

        public async Task ReceiveAsync()
        {
            var brokeredMessage = await _topicSubscriber.ReceiveFromTopicAsync(Topics.Telemetry, SubscriptionName);

            if (brokeredMessage != null)
            {
                var msg = MessageMapper.Map<LuxTelemetry>(brokeredMessage);

                Trace.TraceInformation("Received Lux message.");

                await brokeredMessage.CompleteAsync();
            }
        }
    }
}