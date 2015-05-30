using System.Threading;
using System.Threading.Tasks;

namespace Rumr.Plantduino.Domain.Services
{
    public interface ITopicSubscription
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken token);
    }
}