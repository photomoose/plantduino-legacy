using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Services.Handlers.Telemetry
{
    public class TemperatureTelemetryHandler : IMessageHandler<TemperatureTelemetry>
    {
        private readonly IConfiguration _configuration;
        private readonly ISmsClient _smsClient;
        private readonly INotificationService _notificationService;
        private bool _isColdSpell;
        private DateTime _coldSpellStartUtc;

        public TemperatureTelemetryHandler(IConfiguration configuration, ISmsClient smsClient, INotificationService notificationService)
        {
            _configuration = configuration;
            _smsClient = smsClient;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(TemperatureTelemetry message)
        {
            Trace.TraceInformation("Received {0} message. (Temperature: {1}).", message.GetType().Name, message.Temperature);

            if (message.Temperature <= _configuration.ColdSpellTemp && !_isColdSpell)
            {
                Trace.TraceInformation("{0}: Entering cold spell.", message.DeviceId);

                _isColdSpell = true;
                _coldSpellStartUtc = DateTime.UtcNow;

                await _notificationService.RaiseAsync(
                    new ColdSpellEnteredNotification(
                        message.DeviceId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _coldSpellStartUtc));
            }
            else if (message.Temperature > _configuration.ColdSpellTemp && _isColdSpell)
            {
                var coldSpellEndUtc = DateTime.UtcNow;
                var coldSpellDuration = coldSpellEndUtc - _coldSpellStartUtc;

                Trace.TraceInformation("{0}: Leaving cold spell. (Duration: {1}.)", message.DeviceId, coldSpellDuration);

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