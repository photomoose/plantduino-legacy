using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Domain;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Infrastructure.ServiceBus
{
    public class NotificationService : INotificationService
    {
        private readonly ITopicPublisher _topicPublisher;
        private readonly ITopicSubscriber _topicSubscriber;
        private readonly ITopicManager _topicManager;

        public NotificationService(ITopicPublisher topicPublisher, ITopicSubscriber topicSubscriber, ITopicManager topicManager)
        {
            _topicPublisher = topicPublisher;
            _topicSubscriber = topicSubscriber;
            _topicManager = topicManager;
        }

        public async Task RaiseAsync(NotificationMessage notification)
        {
            var brokeredMessage = MessageMapper.Map(notification);

            await _topicPublisher.SendToTopicAsync(TopicNames.Notifications, brokeredMessage);
        }

        public async Task InitializeAsync<T>() where T : NotificationMessage
        {
            var sd = new SubscriptionDescription(TopicNames.Notifications, typeof(T).Name);

            await _topicManager.CreateSubscriptionAsync(sd, new SqlFilter(string.Format("MessageType = '{0}'", typeof(T).Name)));
        }

        public async Task<T> ReceiveAsync<T>() where T : NotificationMessage
        {
            var brokeredMsg = await _topicSubscriber.ReceiveFromTopicAsync(TopicNames.Notifications, typeof(T).Name);

            return brokeredMsg != null ? MessageMapper.Map<T>(brokeredMsg) : null;
        }
    }
}