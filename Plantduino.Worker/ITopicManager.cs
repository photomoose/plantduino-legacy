using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface ITopicManager
    {
        Task CreateTopicAsync(string topicPath);
        Task CreateTopicAsync(TopicDescription topicDescription);
        Task CreateSubscriptionAsync(SubscriptionDescription subscriptionDescription, Filter subscriptionFilter);
    }
}