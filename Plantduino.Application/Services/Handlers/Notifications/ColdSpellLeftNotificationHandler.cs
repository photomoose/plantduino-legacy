using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Services.Handlers.Notifications
{
    public class ColdSpellLeftNotificationHandler : IMessageHandler<ColdSpellLeftNotification>
    {
        private readonly ISmsClient _smsClient;

        public ColdSpellLeftNotificationHandler(ISmsClient smsClient)
        {
            _smsClient = smsClient;
        }

        public Task HandleAsync(ColdSpellLeftNotification message)
        {
            Trace.TraceInformation("Received {0} message.", message.GetType().Name);

            var leftAtLocal = message.LeftAtUtc.ToLocalTime();

            _smsClient.Send(
                "442071838750", 
                "447742471000", 
                string.Format("{0}: Cold spell over. (Temp: {1}C, Min: {2}C, Duration: {3}).", 
                    leftAtLocal.ToString("t"),
                    message.CurrentTemp,
                    message.MinTemp,
                    message.Duration.ToString(@"h\h\ m\m")));

            return Task.FromResult(0);
        }
    }
}