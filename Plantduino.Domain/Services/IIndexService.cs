using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Telemetry;

namespace Rumr.Plantduino.Domain.Services
{
    public interface IIndexService
    {
        Task IndexAsync<T>(TelemetryIndex<T> message) where T : TelemetryMessage;
    }
}