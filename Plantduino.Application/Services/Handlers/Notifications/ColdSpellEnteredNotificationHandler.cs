using System.Diagnostics;
using System.Threading.Tasks;
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

        public ColdSpellEnteredNotificationHandler(ISmsClient smsClient, IConfiguration configuration)
        {
            _smsClient = smsClient;
            _configuration = configuration;
        }

        public async Task HandleAsync(ColdSpellEnteredNotification message)
        {
            Trace.TraceInformation("Received {0} message.", message.GetType().Name);

            var enteredAtLocal = message.EnteredAtUtc.ToLocalTime();

            _smsClient.Send(_configuration.SmsFrom, _configuration.SmsTo, 
                string.Format(
                "{0}: Entered cold spell. (Temp: {1}C).", 
                enteredAtLocal.ToString("HH:mm"),
                message.CurrentTemp.ToString("f1")));
        }
    }
}