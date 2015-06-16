using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, bool> _isColdSpell = new Dictionary<string, bool>();
        private readonly Dictionary<string, DateTime> _coldSpellEnteredAt = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, double> _minTemp = new Dictionary<string, double>();

        public TemperatureTelemetryHandler(IConfiguration configuration, INotificationService notificationService)
        {
            _configuration = configuration;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(TemperatureTelemetry message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Temperature: {2}}}.", message.DeviceId, message.GetType().Name, message.Temperature);

            var deviceId = message.DeviceId;

            if (_minTemp.ContainsKey(deviceId) && message.Temperature < _minTemp[deviceId])
            {
                _minTemp[deviceId] = message.Temperature;
            }

            if (message.Temperature <= _configuration.ColdSpellTemp && (!_isColdSpell.ContainsKey(deviceId) || !_isColdSpell[deviceId]))
            {
                Trace.TraceInformation("{0}: INFO: Entering cold spell.", message.DeviceId);

                _isColdSpell[deviceId] = true;
                _coldSpellEnteredAt[deviceId] = message.Timestamp;
                _minTemp[deviceId] = message.Temperature;

                await _notificationService.RaiseAsync(
                    new ColdSpellEnteredNotification(
                        message.DeviceId,
                        message.SensorId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _coldSpellEnteredAt[deviceId]));
            }
            else if (message.Temperature > _configuration.ColdSpellTemp && _isColdSpell.ContainsKey(deviceId) && _isColdSpell[deviceId])
            {
                var coldSpellLeftAt = message.Timestamp;
                var coldSpellDuration = coldSpellLeftAt - _coldSpellEnteredAt[deviceId];

                Trace.TraceInformation("{0}: INFO: Leaving cold spell. {{Duration: {1}}}", message.DeviceId, coldSpellDuration);

                _isColdSpell[deviceId] = false;

                await _notificationService.RaiseAsync(
                    new ColdSpellLeftNotification(
                        message.DeviceId,
                        message.SensorId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _minTemp[deviceId],
                        _coldSpellEnteredAt[deviceId],
                        coldSpellLeftAt));
            }
        }
    }
}