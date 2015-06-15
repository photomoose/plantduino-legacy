using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Notifications;
using Rumr.Plantduino.Common;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Tests.Services.Handlers.Notifications
{
    [TestFixture]
    public class ColdSpellLeftNotificationHandlerTests
    {
        private ISmsClient _smsClient;
        private ColdSpellLeftNotificationHandler _handler;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfiguration>();
            _smsClient = Substitute.For<ISmsClient>();
            _handler = new ColdSpellLeftNotificationHandler(_smsClient, _configuration, new DateTimeProvider());
        }

        [Test]
        public async Task When_Message_Is_Handled_Then_Should_Send_Sms()
        {
            const string deviceId = "1";
            const double currentTemp = 1.0;
            const double coldSpellTemp = 2.0;
            const double minTemp = -5.0;
            var enteredAtUtc = new DateTime(2015, 1, 1, 12, 0, 0);
            var leftAtUtc = new DateTime(2015, 1, 1, 13, 30, 10);
            const string from = "0123456789";
            const string to = "9876543210";

            _configuration.SmsFrom.Returns(from);
            _configuration.SmsTo.Returns(to);

            var msg = new ColdSpellLeftNotification(deviceId, currentTemp, coldSpellTemp, minTemp, enteredAtUtc, leftAtUtc);

            await _handler.HandleAsync(msg);

            _smsClient.Received().Send(from, to, "13:30: Cold spell over. (Temp: 1.0C, Min: -5.0C, Duration: 1h 30m).");
        }
    }
}