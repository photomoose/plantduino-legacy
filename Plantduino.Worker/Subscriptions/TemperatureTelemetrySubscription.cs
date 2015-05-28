using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Worker.MessageHandlers;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.Subscriptions
{
    public class TemperatureTelemetrySubscription : ITopicSubscription
    {
        private readonly ITopicManager _topicManager;
        private readonly ITopicSubscriber _topicSubscriber;
        private readonly IEnumerable<IMessageHandler<TemperatureTelemetry>> _handlers;

        public TemperatureTelemetrySubscription(ITopicManager topicManager, ITopicSubscriber topicSubscriber, IEnumerable<IMessageHandler<TemperatureTelemetry>> handlers)
        {
            _topicManager = topicManager;
            _topicSubscriber = topicSubscriber;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            var sd = new SubscriptionDescription(TopicNames.Telemetry, "TemperatureTelemetry");

            await _topicManager.CreateSubscriptionAsync(sd, new SqlFilter(string.Format("MessageType = '{0}'", "TemperatureTelemetry")));
        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var brokeredMsg = await _topicSubscriber.ReceiveFromTopicAsync(TopicNames.Telemetry, "TemperatureTelemetry");

                if (brokeredMsg != null)
                {
                    var telemetryMsg = MessageMapper.Map<TemperatureTelemetry>(brokeredMsg);
                    Parallel.ForEach(_handlers, async h => await h.HandleAsync(telemetryMsg));

                    await brokeredMsg.CompleteAsync();
                }
            }
        }
    }
}