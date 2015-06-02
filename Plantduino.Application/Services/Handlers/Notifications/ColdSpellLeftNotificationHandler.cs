using System.Diagnostics;
using System.Threading.Tasks;
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

        public ColdSpellLeftNotificationHandler(ISmsClient smsClient, IConfiguration configuration)
        {
            _smsClient = smsClient;
            _configuration = configuration;
        }

        public Task HandleAsync(ColdSpellLeftNotification message)
        {
            Trace.TraceInformation("Received {0} message.", message.GetType().Name);

            var leftAtLocal = message.LeftAtUtc.ToLocalTime();

            _smsClient.Send(
                _configuration.SmsFrom, 
                _configuration.SmsTo, 
                string.Format("{0}: Cold spell over. (Temp: {1}C, Min: {2}C, Duration: {3}).", 
                    leftAtLocal.ToString("t"),
                    message.CurrentTemp.ToString("f1"),
                    message.MinTemp.ToString("f1"),
                    message.Duration.ToString(@"h\h\ m\m")));

            return Task.FromResult(0);
        }
    }
}