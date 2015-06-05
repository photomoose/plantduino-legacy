using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Handlers.Telemetry
{
    public class TemperatureTelemetryHandler : IMessageHandler<TemperatureTelemetry>
    {
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private bool _isColdSpell;
        private DateTime _coldSpellStartUtc;

        public TemperatureTelemetryHandler(IConfiguration configuration, INotificationService notificationService)
        {
            _configuration = configuration;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(TemperatureTelemetry message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Temperature: {2}}}.", message.DeviceId, message.GetType().Name, message.Temperature);

            if (message.Temperature <= _configuration.ColdSpellTemp && !_isColdSpell)
            {
                Trace.TraceInformation("{0}: INFO: Entering cold spell.", message.DeviceId);

                _isColdSpell = true;
                _coldSpellStartUtc = message.Timestamp;

                await _notificationService.RaiseAsync(
                    new ColdSpellEnteredNotification(
                        message.DeviceId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _coldSpellStartUtc));
            }
            else if (message.Temperature > _configuration.ColdSpellTemp && _isColdSpell)
            {
                var coldSpellEndUtc = message.Timestamp;
                var coldSpellDuration = coldSpellEndUtc - _coldSpellStartUtc;

                Trace.TraceInformation("{0}: INFO: Leaving cold spell. {{Duration: {1}}}", message.DeviceId, coldSpellDuration);

                _isColdSpell = false;

                await _notificationService.RaiseAsync(
                    new ColdSpellLeftNotification(
                        message.DeviceId,
                        message.Temperature,
                        _configuration.ColdSpellTemp, 0, _coldSpellStartUtc,
                        coldSpellEndUtc));
            }
        }
    }
}