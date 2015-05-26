using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface ITopicPublisher
    {
        Task SendToTopicAsync(string topicPath, BrokeredMessage message);
    }
}