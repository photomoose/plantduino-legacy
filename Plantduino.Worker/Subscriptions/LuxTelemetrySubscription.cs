using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Worker.MessageHandlers;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.Subscriptions
{
    public class LuxTelemetrySubscription : ITopicSubscription
    {
        private readonly ITopicManager _topicManager;
        private readonly ITopicSubscriber _topicSubscriber;
        private readonly IEnumerable<IMessageHandler<LuxTelemetry>> _handlers;

        public LuxTelemetrySubscription(ITopicManager topicManager, ITopicSubscriber topicSubscriber, IEnumerable<IMessageHandler<LuxTelemetry>> handlers)
        {
            _topicManager = topicManager;
            _topicSubscriber = topicSubscriber;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            var sd = new SubscriptionDescription(TopicNames.Telemetry, "LuxTelemetry");

            await _topicManager.CreateSubscriptionAsync(sd, new SqlFilter(string.Format("MessageType = '{0}'", "LuxTelemetry")));
        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var brokeredMsg = await _topicSubscriber.ReceiveFromTopicAsync(TopicNames.Telemetry, "LuxTelemetry");

                if (brokeredMsg != null)
                {
                    var telemetryMsg = MessageMapper.Map<LuxTelemetry>(brokeredMsg);
                    Parallel.ForEach(_handlers, async h => await h.HandleAsync(telemetryMsg));

                    await brokeredMsg.CompleteAsync();
                }
            }
        }
    }
}