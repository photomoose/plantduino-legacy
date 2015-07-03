using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Common;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Repositories;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Services.Handlers.Notifications
{
    public class ColdSpellEnteredNotificationHandler : IMessageHandler<ColdSpellEnteredNotification>
    {
        private readonly IColdSpellRepository _coldSpellRepository;
        private readonly ISmsClient _smsClient;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private ITimeZone _timeZone;

        public ColdSpellEnteredNotificationHandler(IColdSpellRepository coldSpellRepository, ISmsClient smsClient, IConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            _coldSpellRepository = coldSpellRepository;
            _smsClient = smsClient;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _timeZone = new GmtTimeZone();
        }

        public async Task HandleAsync(ColdSpellEnteredNotification message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Temp: {2}, EnteredAt: {3}}}.", message.DeviceId, message.GetType().Name, message.CurrentTemp, message.EnteredAt);

            var coldSpell = await _coldSpellRepository.GetAsync(message.DeviceId, message.SensorId);
            var utcNow = _dateTimeProvider.UtcNow();

            if (coldSpell != null)
            {
                Trace.TraceInformation("{0}: INFO: Last cold spell for sensor {1} occurred at {2}.", message.DeviceId, message.SensorId, coldSpell.AlertedAt);

                if (utcNow - coldSpell.AlertedAt > TimeSpan.FromHours(6))
                {
                    SendAlert(message);

                    coldSpell.AlertedAt = utcNow;
                    await _coldSpellRepository.SaveAsync(coldSpell);                 
                }
            }
            else
            {
                SendAlert(message);

                coldSpell = new ColdSpell
                {
                    DeviceId = message.DeviceId,
                    SensorId = message.SensorId,
                    AlertedAt = utcNow
                };

                await _coldSpellRepository.SaveAsync(coldSpell);                 
            }
        }

        private void SendAlert(ColdSpellEnteredNotification message)
        {
            var enteredAtLocal = _timeZone.ToLocalTime(message.EnteredAt);

            _smsClient.Send(_configuration.SmsFrom, _configuration.SmsTo,
                string.Format(
                    "{0}: Entered cold spell. (Temp: {1}C).",
                    enteredAtLocal.ToString("HH:mm"),
                    message.CurrentTemp.ToString("f1")));

            Trace.TraceInformation("{0}: INFO: Sent SMS for ColdSpellEnteredNotification.", message.DeviceId);
        }
    }
}