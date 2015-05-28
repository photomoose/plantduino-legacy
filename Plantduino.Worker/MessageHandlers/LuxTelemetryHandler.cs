using System;
using System.Threading.Tasks;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.MessageHandlers
{
    public class LuxTelemetryHandler : IMessageHandler<LuxTelemetry>
    {
        public Task HandleAsync(LuxTelemetry message)
        {
            return Task.FromResult(0);
        }
    }
}