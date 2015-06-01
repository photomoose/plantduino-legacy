using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Services.Handlers.Notifications
{
    public class ColdSpellEnteredNotificationHandler : IMessageHandler<ColdSpellEnteredNotification>
    {
        private readonly ISmsClient _smsClient;

        public ColdSpellEnteredNotificationHandler(ISmsClient smsClient)
        {
            _smsClient = smsClient;
        }

        public Task HandleAsync(ColdSpellEnteredNotification message)
        {
            Trace.TraceInformation("Received {0} message.", message.GetType().Name);

            var enteredAtLocal = message.EnteredAtUtc.ToLocalTime();

            _smsClient.Send("442071838750", "447742471000", 
                string.Format(
                "{0}: Entered cold spell. (Temp: {1}C).", 
                enteredAtLocal.ToString("t"),
                message.CurrentTemp));

            return Task.FromResult(0);
        }
    }
}