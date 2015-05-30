using Rumr.Plantduino.Domain.Sms;
using Twilio;

namespace Plantduino.Infrastructure.Twilio
{
    public class TwilioSmsClient : ISmsClient
    {
        private readonly ITwilioAccount _twilioAccount;

        public TwilioSmsClient(ITwilioAccount twilioAccount)
        {
            _twilioAccount = twilioAccount;
        }

        public void Send(string from, string to, string text)
        {
            var client = new TwilioRestClient(_twilioAccount.AccountSid, _twilioAccount.AuthToken);

            client.SendMessage(from, to, text);
        }
    }
}