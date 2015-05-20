using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Worker.Handlers;
using Rumr.Plantduino.Worker.Messages;
using Rumr.Plantduino.Worker.Sms;

namespace Rumr.Plantduino.Worker.Tests
{
    [TestFixture]
    public class ColdPeriodBeginHandlerTests
    {
        public class ColdPeriodBeginHandlerScenario
        {
            protected ISmsClient SmsClient;

            protected ColdPeriodHandler CreateHandler()
            {
                SmsClient = Substitute.For<ISmsClient>();
                
                return new ColdPeriodHandler(SmsClient);
            }

            protected ColdPeriodEvent CreateMessage()
            {
                return new ColdPeriodEvent();
            }
        }

        public class WhenAMessageIsReceived : ColdPeriodBeginHandlerScenario
        {
            [SetUp]
            public void SetUp()
            {
                var handler = CreateHandler();
                var msg = CreateMessage();

                handler.Process(msg);
            }

            [Test]
            public void ThenAnSmsShouldBeSent()
            {
                SmsClient.Received().Send("", "", "Entered cold period.");
            }
        }
    }
}