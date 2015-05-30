using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Domain;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Infrastructure.ServiceBus
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ITopicSubscriber _topicSubscriber;
        private readonly ITopicManager _topicManager;

        public TelemetryService(ITopicSubscriber topicSubscriber, ITopicManager topicManager)
        {
            _topicSubscriber = topicSubscriber;
            _topicManager = topicManager;
        }

        public async Task InitializeAsync<T>() where T : TelemetryMessage
        {
            var sd = new SubscriptionDescription(TopicNames.Telemetry, typeof(T).Name);

            await _topicManager.CreateSubscriptionAsync(sd, new SqlFilter(string.Format("MessageType = '{0}'", typeof(T).Name)));
        }

        public async Task<T> ReceiveAsync<T>() where T : TelemetryMessage
        {
            var brokeredMsg = await _topicSubscriber.ReceiveFromTopicAsync(TopicNames.Telemetry, typeof(T).Name);

            return brokeredMsg != null ? MessageMapper.Map<T>(brokeredMsg) : null;
        }
    }
}