using System.Threading;
using System.Threading.Tasks;

namespace Rumr.Plantduino.Worker.Subscriptions
{
    public interface ITopicSubscription
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken token);
    }
}