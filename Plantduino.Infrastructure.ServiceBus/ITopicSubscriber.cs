using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Infrastructure.ServiceBus
{
    public interface ITopicSubscriber
    {
        Task<BrokeredMessage> ReceiveFromTopicAsync(string topicPath, string subscriptionName);
    }
}