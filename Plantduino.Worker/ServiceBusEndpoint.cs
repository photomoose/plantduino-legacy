using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface IQueueReceiver
    {
        Task<BrokeredMessage> ReceiveFromQueueAsync(string queuePath);
    }

    public class ServiceBusEndpoint : ITopicPublisher, ITopicManager, ITopicSubscriber, IQueueManager, IQueuePublisher, IQueueReceiver
    {
        private readonly MessagingFactory _factory;
        private readonly NamespaceManager _namespaceManager;

        public ServiceBusEndpoint(IServiceBusConfiguration configuration)
        {
            _factory = MessagingFactory.CreateFromConnectionString(configuration.ConnectionString);
            _namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
        }

        public async Task CreateQueueAsync(string queuePath)
        {
            var qd = new QueueDescription(queuePath);

            await CreateQueueAsync(qd);
        }

        public async Task CreateQueueAsync(QueueDescription queueDescription)
        {
            var exists = await _namespaceManager.QueueExistsAsync(queueDescription.Path);

            if (!exists)
            {
                await _namespaceManager.CreateQueueAsync(queueDescription);
            }
        }

        public async Task<BrokeredMessage> ReceiveFromQueueAsync(string queuePath)
        {
            var client = _factory.CreateQueueClient(queuePath);

            return await client.ReceiveAsync();
        }

        public async Task SendToQueueAsync(string queuePath, Message message)
        {
            var brokeredMessage = MessageSerializer.Map(message);

            var client = _factory.CreateQueueClient(queuePath);

            await client.SendAsync(brokeredMessage);
        }

        public async Task CreateTopicAsync(string topicPath)
        {
            var td = new TopicDescription(topicPath);

            await CreateTopicAsync(td);
        }

        public async Task CreateTopicAsync(TopicDescription topicDescription)
        {
            var exists = await _namespaceManager.TopicExistsAsync(topicDescription.Path);

            if (!exists)
            {
                await _namespaceManager.CreateTopicAsync(topicDescription);
            }
        }

        public async Task CreateSubscriptionAsync(SubscriptionDescription subscriptionDescription, Filter subscriptionFilter)
        {
            var exists =
                await
                    _namespaceManager.SubscriptionExistsAsync(subscriptionDescription.TopicPath,
                        subscriptionDescription.Name);

            if (!exists)
            {
                await _namespaceManager.CreateSubscriptionAsync(subscriptionDescription, subscriptionFilter);
            }
        }

        public async Task SendToTopicAsync(string topicPath, Message message)
        {
            var msg = MessageSerializer.Map(message);

            var client = _factory.CreateTopicClient(topicPath);

            await client.SendAsync(msg);
        }

        public async Task<BrokeredMessage> ReceiveFromTopicAsync(string topicPath, string subscriptionName)
        {
            var client = _factory.CreateSubscriptionClient(topicPath, subscriptionName);

            return await client.ReceiveAsync();
        }
    }
}