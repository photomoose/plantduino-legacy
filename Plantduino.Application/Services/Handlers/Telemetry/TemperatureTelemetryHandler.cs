using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain;
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
        private readonly INotificationService _notificationService;
        private readonly IIndexService _indexService;
        private bool _isColdSpell;
        private DateTime _coldSpellStartUtc;

        public TemperatureTelemetryHandler(IConfiguration configuration, INotificationService notificationService, IIndexService indexService)
        {
            _configuration = configuration;
            _notificationService = notificationService;
            _indexService = indexService;
        }

        public async Task HandleAsync(TemperatureTelemetry message)
        {
            Trace.TraceInformation("Received {0} message. (Temperature: {1}).", message.GetType().Name, message.Temperature);

            if (message.Temperature <= _configuration.ColdSpellTemp && !_isColdSpell)
            {
                Trace.TraceInformation("{0}: Entering cold spell.", message.DeviceId);

                _isColdSpell = true;
                _coldSpellStartUtc = message.EnqueuedTimeUtc;

                await _notificationService.RaiseAsync(
                    new ColdSpellEnteredNotification(
                        message.DeviceId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _coldSpellStartUtc));
            }
            else if (message.Temperature > _configuration.ColdSpellTemp && _isColdSpell)
            {
                var coldSpellEndUtc = message.EnqueuedTimeUtc;
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

            var index = new TemperatureTelemetryIndex(message.Temperature, message.DeviceId, message.EnqueuedTimeUtc);

            await _indexService.IndexAsync(index);
        }
    }
}