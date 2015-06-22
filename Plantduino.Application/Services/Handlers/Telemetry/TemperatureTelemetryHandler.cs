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

            var sensorId = message.SensorId;

            if (_minTemp.ContainsKey(sensorId) && message.Temperature < _minTemp[sensorId])
            {
                _minTemp[sensorId] = message.Temperature;
            }

            if (message.Temperature <= _configuration.ColdSpellTemp && (!_isColdSpell.ContainsKey(sensorId) || !_isColdSpell[sensorId]))
            {
                Trace.TraceInformation("{0}: INFO: Entering cold spell.", message.DeviceId);

                _isColdSpell[sensorId] = true;
                _coldSpellEnteredAt[sensorId] = message.Timestamp;
                _minTemp[sensorId] = message.Temperature;

                await _notificationService.RaiseAsync(
                    new ColdSpellEnteredNotification(
                        message.DeviceId,
                        message.SensorId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _coldSpellEnteredAt[sensorId]));
            }
            else if (message.Temperature > _configuration.ColdSpellTemp && _isColdSpell.ContainsKey(sensorId) && _isColdSpell[sensorId])
            {
                var coldSpellLeftAt = message.Timestamp;
                var coldSpellDuration = coldSpellLeftAt - _coldSpellEnteredAt[sensorId];

                Trace.TraceInformation("{0}: INFO: Leaving cold spell. {{Duration: {1}}}", message.DeviceId, coldSpellDuration);

                _isColdSpell[sensorId] = false;

                await _notificationService.RaiseAsync(
                    new ColdSpellLeftNotification(
                        message.DeviceId,
                        message.SensorId,
                        message.Temperature,
                        _configuration.ColdSpellTemp,
                        _minTemp[sensorId],
                        _coldSpellEnteredAt[sensorId],
                        coldSpellLeftAt));
            }
        }
    }
}