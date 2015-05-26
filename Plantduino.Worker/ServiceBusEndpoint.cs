using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public class ServiceBusEndpoint : ITopicPublisher, ITopicManager, ITopicSubscriber
    {
        private readonly MessagingFactory _factory;
        private readonly NamespaceManager _namespaceManager;

        public ServiceBusEndpoint(IServiceBusConfiguration configuration)
        {
            _factory = MessagingFactory.CreateFromConnectionString(configuration.ConnectionString);
            _namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
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

        public async Task SendToTopicAsync(string topicPath, BrokeredMessage message)
        {
            var client = _factory.CreateTopicClient(topicPath);

            await client.SendAsync(message);
        }

        public async Task<BrokeredMessage> ReceiveFromTopicAsync(string topicPath, string subscriptionName)
        {
            var client = _factory.CreateSubscriptionClient(topicPath, subscriptionName);

            return await client.ReceiveAsync();
        }
    }
}