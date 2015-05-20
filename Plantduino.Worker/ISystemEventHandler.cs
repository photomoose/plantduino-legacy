using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface ISystemEventHandler
    {
        Filter SubscriptionFilter { get; }
        string Name { get; }
        Task ReceiveAsync();
    }
}