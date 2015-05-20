using System.Threading.Tasks;

namespace Rumr.Plantduino.Worker
{
    public interface ITopicPublisher
    {
        Task SendToTopicAsync(string topicPath, Message message);
    }
}