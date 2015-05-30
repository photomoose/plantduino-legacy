//using System;
//using System.Threading.Tasks;
//using NSubstitute;
//using NUnit.Framework;
//using Rumr.Plantduino.Domain.Messages.Notifications;
//using Rumr.Plantduino.Domain.Sms;

//namespace Rumr.Plantduino.Worker.Tests
//{
//    [TestFixture]
//    public class ColdSpellLeftNotificationHandlerTests
//    {
//        private const int DeviceId = 1;
//        private const double CurrentTemp = 5.1;
//        private const double ColdSpellTemp = 2.0;
//        private const double MinTemp = -1.5;
//        private ISmsClient _smsClient;
//        private ColdSpellLeftNotificationHandler _handler;
//        private readonly DateTime _enteredAtUtc = new DateTime(2015, 1, 1, 23, 0, 0, DateTimeKind.Utc);
//        private DateTime _leftAtUtc = new DateTime(2015, 1, 2, 8, 15, 35, DateTimeKind.Utc);

//        [SetUp]
//        public void SetUp()
//        {
//            _smsClient = Substitute.For<ISmsClient>();
//            _handler = new ColdSpellLeftNotificationHandler(_smsClient);
//        }

//        [Test]
//        public async Task When_Message_Handled_Should_Send_Sms()
//        {
//            var message = CreateMessage();

//            await _handler.HandleAsync(message);

//            _smsClient.Received().Send(Arg.Any<string>(), Arg.Any<string>(), "08:15: Cold spell over. (Temp: 5.1C, Min: -1.5C, Duration: 9h 15m).");
//        }

//        [Test]
//        public async Task When_Message_Handled_Should_Send_Sms_Using_Local_Time()
//        {
//            _leftAtUtc = new DateTime(2015, 6, 2, 8, 15, 35, DateTimeKind.Utc);

//            var message = CreateMessage();

//            await _handler.HandleAsync(message);

//            _smsClient.Received().Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Is<string>(s => s.Contains("09:15:")));
//        }

//        private ColdSpellLeftNotification CreateMessage()
//        {
//            return new ColdSpellLeftNotification(DeviceId, CurrentTemp, ColdSpellTemp, MinTemp, _enteredAtUtc, _leftAtUtc);
//        }
//    }
//}