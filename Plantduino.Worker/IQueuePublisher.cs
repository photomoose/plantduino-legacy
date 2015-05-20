using System.Threading.Tasks;

namespace Rumr.Plantduino.Worker
{
    public interface IQueuePublisher
    {
        Task SendToQueueAsync(string queuePath, Message message);
    }
}