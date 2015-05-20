using NUnit.Framework;
using Rumr.Plantduino.Worker.Sms;

namespace Rumr.Plantduino.Worker.Tests
{
    [TestFixture]
    public class TwilioSmsClientTests
    {
        [Ignore("It costs money")]
        [Test]
        public void ClientShouldSendSms()
        {
            var account = new Configuration();
            var client = new TwilioSmsClient(account);
            client.Send("442071838750", "447742471000", "Test message");
        }
    }
}