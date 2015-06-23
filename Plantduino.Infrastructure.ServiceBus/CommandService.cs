using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Domain;
using Rumr.Plantduino.Domain.Messages.Commands;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Infrastructure.ServiceBus
{
    public class CommandService : ICommandService
    {
        private readonly ITopicPublisher _topicPublisher;
        private readonly ITopicManager _topicManager;

        public CommandService(ITopicPublisher topicPublisher, ITopicSubscriber topicSubscriber, ITopicManager topicManager)
        {
            _topicPublisher = topicPublisher;
            _topicManager = topicManager;
        }

        public async Task RaiseAsync(CommandMessage command)
        {
            var brokeredMessage = MessageMapper.Map(command);

            await _topicPublisher.SendToTopicAsync(TopicNames.Commands, brokeredMessage);
        }

        public async Task InitializeAsync<T>() where T : CommandMessage
        {
            var sd = new SubscriptionDescription(TopicNames.Notifications, typeof(T).Name);

            await _topicManager.CreateSubscriptionAsync(sd, new SqlFilter(string.Format("MessageType = '{0}'", typeof(T).Name)));
        }
    }
}