using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Handlers.Telemetry
{
    public class MoistureTelemetryHandler : IMessageHandler<MoistureTelemetry>
    {
        public Task HandleAsync(MoistureTelemetry message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Moisture: {2}}}.", message.DeviceId, message.GetType().Name, message.Moisture);

            return Task.FromResult(0);
        }
    }
}