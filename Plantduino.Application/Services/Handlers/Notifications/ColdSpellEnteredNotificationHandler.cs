using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Common;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Services.Handlers.Notifications
{
    public class ColdSpellEnteredNotificationHandler : IMessageHandler<ColdSpellEnteredNotification>
    {
        private readonly ISmsClient _smsClient;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ColdSpellEnteredNotificationHandler(ISmsClient smsClient, IConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            _smsClient = smsClient;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task HandleAsync(ColdSpellEnteredNotification message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Temp: {2}, EnteredAt: {3}}}.", message.DeviceId, message.GetType().Name, message.CurrentTemp, message.EnteredAt);

            var enteredAtLocal = _dateTimeProvider.ToLocalTime(message.EnteredAt);

            _smsClient.Send(_configuration.SmsFrom, _configuration.SmsTo, 
                string.Format(
                "{0}: Entered cold spell. (Temp: {1}C).", 
                enteredAtLocal.ToString("HH:mm"),
                message.CurrentTemp.ToString("f1")));

            Trace.TraceInformation("{0}: INFO: Sent SMS for ColdSpellEnteredNotification.", message.DeviceId);
        }
    }
}