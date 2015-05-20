using Twilio;

namespace Rumr.Plantduino.Worker.Sms
{
    public class TwilioSmsClient : ISmsClient
    {
        private readonly ITwilioAccount _twilioAccount;
        private const string AccountSid = "ACcf256a910d8b4c15565d35e13bb36b04";
        private const string AuthToken = "a98b8b213b8a05f5b19e0950604199d0";

        public TwilioSmsClient(ITwilioAccount twilioAccount)
        {
            _twilioAccount = twilioAccount;
        }

        public void Send(string from, string to, string text)
        {
            var client = new TwilioRestClient(AccountSid, AuthToken);

            client.SendMessage(from, to, text);
        }
    }
}