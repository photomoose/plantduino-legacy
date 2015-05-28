using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker
{
    public interface ITopicPublisher
    {
        Task SendToTopicAsync(string topicPath, BrokeredMessage message);
        Task SendToTopicAsync(string topicPath, Message message);
    }
}