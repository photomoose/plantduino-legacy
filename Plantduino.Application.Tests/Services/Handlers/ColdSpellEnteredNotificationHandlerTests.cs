using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Notifications;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Sms;

namespace Plantduino.Application.Tests.Services.Handlers
{
    [TestFixture]
    public class ColdSpellEnteredNotificationHandlerTests
    {
        private ISmsClient _smsClient;
        private ColdSpellEnteredNotificationHandler _handler;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfiguration>();
            _smsClient = Substitute.For<ISmsClient>();
            _handler = new ColdSpellEnteredNotificationHandler(_smsClient, _configuration);
        }

        [Test]
        public async Task When_Message_Is_Handled_Then_Should_Send_Sms()
        {
            const int deviceId = 1;
            const double currentTemp = 1.5;
            const double coldSpellTemp = 2.0;
            var enteredAtUtc = new DateTime(2015, 1, 1, 12, 0, 0);
            const string from = "0123456789";
            const string to = "9876543210";

            _configuration.SmsFrom.Returns(from);
            _configuration.SmsTo.Returns(to);

            var msg = new ColdSpellEnteredNotification(deviceId, currentTemp, coldSpellTemp, enteredAtUtc);

            await _handler.HandleAsync(msg);

            _smsClient.Received().Send(from, to, "12:00: Entered cold spell. (Temp: 1.5C).");
        }

        [Test]
        public async Task When_Message_Is_Handled_Then_Should_Send_Sms_Using_Local_Time()
        {
            const int deviceId = 1;
            const double currentTemp = 1.0;
            const double coldSpellTemp = 2.0;
            var enteredAtUtc = new DateTime(2015, 6, 1, 11, 0, 0);  // 12:00 BST
            const string from = "0123456789";
            const string to = "9876543210";

            _configuration.SmsFrom.Returns(from);
            _configuration.SmsTo.Returns(to);

            var msg = new ColdSpellEnteredNotification(deviceId, currentTemp, coldSpellTemp, enteredAtUtc);

            await _handler.HandleAsync(msg);

            _smsClient.Received().Send(from, to, "12:00: Entered cold spell. (Temp: 1.0C).");
        }
    }
}