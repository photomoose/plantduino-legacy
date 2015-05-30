using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Handlers.Telemetry
{
    public class LuxTelemetryHandler : IMessageHandler<LuxTelemetry>
    {
        public Task HandleAsync(LuxTelemetry message)
        {
            Trace.TraceInformation("Received {0} message.", message.GetType().Name);

            return Task.FromResult(0);
        }
    }
}