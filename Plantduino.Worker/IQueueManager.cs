using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface IQueueManager
    {
        Task CreateQueueAsync(string queuePath);
        Task CreateQueueAsync(QueueDescription queueDescription);
    }
}