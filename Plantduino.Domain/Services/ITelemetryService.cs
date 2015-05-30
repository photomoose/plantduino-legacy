using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Telemetry;

namespace Rumr.Plantduino.Domain.Services
{
    public interface ITelemetryService
    {
        Task<T> ReceiveAsync<T>() where T : TelemetryMessage;

        Task InitializeAsync<T>() where T : TelemetryMessage;
    }
}