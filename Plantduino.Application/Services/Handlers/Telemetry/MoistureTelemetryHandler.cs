using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Commands;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Handlers.Telemetry
{
    public class MoistureTelemetryHandler : IMessageHandler<MoistureTelemetry>
    {
        private readonly ICommandService _commandService;
        private readonly IConfiguration _configuration;

        public MoistureTelemetryHandler(ICommandService commandService, IConfiguration configuration)
        {
            _commandService = commandService;
            _configuration = configuration;
        }

        public async Task HandleAsync(MoistureTelemetry message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Moisture: {2}}}.", message.DeviceId, message.GetType().Name, message.Moisture);

            if (message.Moisture <= _configuration.MoistureMin)
            {
                await _commandService.RaiseAsync(new IrrigateCommand(message.DeviceId, message.SensorId, _configuration.IrrigationDuration.TotalMilliseconds));
            }
        }
    }
}