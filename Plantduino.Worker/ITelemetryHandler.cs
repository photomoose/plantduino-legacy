using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public interface ITelemetryHandler
    {
        string SubscriptionName { get; }
        Filter SubscriptionFilter { get; }
        Task ReceiveAsync();
    }
}