using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface ITopicSubscriber
    {
        Task<BrokeredMessage> ReceiveFromTopicAsync(string topicPath, string subscriptionName);
    }
}