//using System;
//using System.Threading.Tasks;
//using NSubstitute;
//using NUnit.Framework;
//using Rumr.Plantduino.Worker.MessageHandlers;
//using Rumr.Plantduino.Worker.Notifications;
//using Rumr.Plantduino.Worker.Sms;

//namespace Rumr.Plantduino.Worker.Tests
//{
//    [TestFixture]
//    public class ColdSpellEnteredNotificationHandlerTests
//    {
//        private const int DeviceId = 1;
//        private const double CurrentTemp = 1.1;
//        private const double ColdSpellTemp = 2.0;
//        private ISmsClient _smsClient;
//        private ColdSpellEnteredNotificationHandler _handler;
//        private DateTime _enteredAtUtc = new DateTime(2015, 1, 1, 23, 0, 0, DateTimeKind.Utc);

//        [SetUp]
//        public void SetUp()
//        {
//            _smsClient = Substitute.For<ISmsClient>();
//            _handler = new ColdSpellEnteredNotificationHandler(_smsClient);
//        }

//        [Test]
//        public async Task When_Message_Handled_Should_Send_Sms()
//        {
//            var message = CreateMessage();

//            await _handler.HandleAsync(message);

//            _smsClient.Received().Send(Arg.Any<string>(), Arg.Any<string>(), "23:00: Entered cold spell. (Temp: 1.1C).");
//        }

//        [Test]
//        public async Task When_Message_Handled_Should_Send_Sms_Using_Local_Time()
//        {
//            _enteredAtUtc = new DateTime(2015, 6, 2, 8, 15, 35, DateTimeKind.Utc);

//            var message = CreateMessage();

//            await _handler.HandleAsync(message);

//            _smsClient.Received().Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Is<string>(s => s.Contains("09:15:")));
//        }

//        private ColdSpellEnteredNotification CreateMessage()
//        {
//            return new ColdSpellEnteredNotification(DeviceId, CurrentTemp, ColdSpellTemp, _enteredAtUtc);
//        }
//    }
//}