using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Common;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Services.Handlers.Notifications
{
    public class ColdSpellLeftNotificationHandler : IMessageHandler<ColdSpellLeftNotification>
    {
        private readonly ISmsClient _smsClient;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ColdSpellLeftNotificationHandler(ISmsClient smsClient, IConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            _smsClient = smsClient;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task HandleAsync(ColdSpellLeftNotification message)
        {
            Trace.TraceInformation("{0}: HANDLE: {1} {{Temp: {2}, LeftAt: {3}, Duration: {4}}}.", message.DeviceId, message.GetType().Name, message.CurrentTemp, message.LeftAt, message.Duration);

            var leftAtLocal = _dateTimeProvider.ToLocalTime(message.LeftAt);

            _smsClient.Send(
                _configuration.SmsFrom, 
                _configuration.SmsTo, 
                string.Format("{0}: Cold spell over. (Temp: {1}C, Min: {2}C, Duration: {3}).", 
                    leftAtLocal.ToString("HH:mm"),
                    message.CurrentTemp.ToString("f1"),
                    message.MinTemp.ToString("f1"),
                    message.Duration.ToString(@"h\h\ m\m")));

            Trace.TraceInformation("{0}: INFO: Sent SMS for ColdSpellLeftNotification.", message.DeviceId);

            return Task.FromResult(0);
        }
    }
}