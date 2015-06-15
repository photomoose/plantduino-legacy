using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Notifications;
using Rumr.Plantduino.Common;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;
using Rumr.Plantduino.Domain.Sms;

namespace Rumr.Plantduino.Application.Tests.Services.Handlers.Notifications
{
    [TestFixture]
    public class ColdSpellEnteredNotificationHandlerTests
    {
        public abstract class ColdSpellEnteredNotificationHandlerFixture
        {
            protected const string DeviceId = "1";
            protected ISmsClient SmsClient;
            protected ColdSpellEnteredNotificationHandler Handler;
            protected IConfiguration Configuration;

            [SetUp]
            public void SetUp()
            {
                Configuration = Substitute.For<IConfiguration>();
                SmsClient = Substitute.For<ISmsClient>();
                Handler = new ColdSpellEnteredNotificationHandler(SmsClient, Configuration, new DateTimeProvider());

                Before();
            }

            protected virtual void Before()
            {
            }

            protected static ColdSpellEnteredNotification CreateNotification(double currentTemp, double coldSpellTemp, DateTime enteredAtUtc)
            {
                return new ColdSpellEnteredNotification(DeviceId, currentTemp, coldSpellTemp, enteredAtUtc);
            }
        }

        public class Given_Any_Scenario : ColdSpellEnteredNotificationHandlerFixture
        {
            const string From = "0123456789";
            const string To = "9876543210";

            protected override void Before()
            {
                Configuration.SmsFrom.Returns(From);
                Configuration.SmsTo.Returns(To);
            }

            [Test]
            public async Task When_Notification_Is_Handled_Then_Should_Send_Sms()
            {
                const double currentTemp = 1.5;
                const double coldSpellTemp = 2.0;
                var enteredAtUtc = new DateTime(2015, 1, 1, 12, 0, 0);
                
                var notification = CreateNotification(currentTemp, coldSpellTemp, enteredAtUtc);

                await Handler.HandleAsync(notification);

                SmsClient.Received().Send(From, To, "12:00: Entered cold spell. (Temp: 1.5C).");
            }

            [Test]
            public async Task When_Notification_Is_Handled_Then_Should_Send_Sms_Using_Local_Time()
            {
                const double currentTemp = 1.0;
                const double coldSpellTemp = 2.0;
                var enteredAtUtc = new DateTime(2015, 6, 1, 11, 0, 0, DateTimeKind.Utc); // 12:00 BST

                var notification = CreateNotification(currentTemp, coldSpellTemp, enteredAtUtc);

                await Handler.HandleAsync(notification);

                SmsClient.Received().Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Is<string>(msg => msg.Contains("12:00:")));
            }
        }
    }
}