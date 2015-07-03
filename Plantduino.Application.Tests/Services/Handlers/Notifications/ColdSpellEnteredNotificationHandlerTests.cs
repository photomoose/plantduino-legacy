using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Notifications;
using Rumr.Plantduino.Common;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Repositories;
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
            protected const string SensorId = "inside";
            protected ISmsClient SmsClient;
            protected ColdSpellEnteredNotificationHandler Handler;
            protected IConfiguration Configuration;
            protected IColdSpellRepository ColdSpellRepository;
            protected IDateTimeProvider DateTimeProvider;

            [SetUp]
            public void SetUp()
            {
                Configuration = Substitute.For<IConfiguration>();
                SmsClient = Substitute.For<ISmsClient>();
                ColdSpellRepository = Substitute.For<IColdSpellRepository>();
                DateTimeProvider = Substitute.For<IDateTimeProvider>();
                
                Before();
                
                Handler = new ColdSpellEnteredNotificationHandler(ColdSpellRepository, SmsClient, Configuration, DateTimeProvider);
            }

            protected virtual void Before()
            {
            }

            protected static ColdSpellEnteredNotification CreateNotification(double currentTemp, double coldSpellTemp, DateTime enteredAtUtc)
            {
                return new ColdSpellEnteredNotification(DeviceId, SensorId, currentTemp, coldSpellTemp, enteredAtUtc);
            }
        }

        public class Given_No_Previous_Cold_Spell : ColdSpellEnteredNotificationHandlerFixture
        {
            private const string From = "0123456789";
            private const string To = "9876543210";
            private readonly DateTime _utcNow = new DateTime(2015, 1, 1, 6, 0, 1, DateTimeKind.Utc);

            protected override void Before()
            {
                Configuration.SmsFrom.Returns(From);
                Configuration.SmsTo.Returns(To);

                ColdSpellRepository.GetAsync(Arg.Any<string>(), Arg.Any<string>())
                    .Returns(Task.FromResult<ColdSpell>(null));
                DateTimeProvider.UtcNow().Returns(_utcNow);
            }

            [Test]
            public async Task When_Notification_Is_Handled_Then_Should_Send_Sms()
            {
                const double currentTemp = 1.5;
                const double coldSpellTemp = 2.0;
                var enteredAtUtc = new DateTime(2015, 1, 1, 12, 0, 0, DateTimeKind.Utc);

                var notification = CreateNotification(currentTemp, coldSpellTemp, enteredAtUtc);

                await Handler.HandleAsync(notification);

                SmsClient.Received().Send(From, To, "12:00: Entered cold spell. (Temp: 1.5C).");
            }

            [Test]
            public async Task When_Notification_Is_Handled_Then_Should_Add_Entity_To_Repository()
            {
                const double currentTemp = 1.5;
                const double coldSpellTemp = 2.0;
                var enteredAtUtc = new DateTime(2015, 1, 1, 12, 0, 0, DateTimeKind.Utc);

                var notification = CreateNotification(currentTemp, coldSpellTemp, enteredAtUtc);

                await Handler.HandleAsync(notification);

                ColdSpellRepository.Received().SaveAsync(Arg.Is<ColdSpell>(cs => 
                    cs.AlertedAt == _utcNow &&
                    cs.DeviceId == DeviceId &&
                    cs.SensorId == SensorId));
            }

        }

        public class Given_Alert_Not_Recently_Sent : ColdSpellEnteredNotificationHandlerFixture
        {
            const string From = "0123456789";
            const string To = "9876543210";
            private readonly DateTime _lastAlertedAt = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            private readonly DateTime _utcNow = new DateTime(2015, 1, 1, 6, 0, 1, DateTimeKind.Utc);

            protected override void Before()
            {
                Configuration.SmsFrom.Returns(From);
                Configuration.SmsTo.Returns(To);

                var coldSpell = new ColdSpell {AlertedAt = _lastAlertedAt};

                ColdSpellRepository.GetAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(coldSpell));
                DateTimeProvider.UtcNow().Returns(_utcNow);
            }

            [Test]
            public async Task When_Notification_Is_Handled_Then_Should_Send_Sms()
            {
                const double currentTemp = 1.5;
                const double coldSpellTemp = 2.0;
                var enteredAtUtc = new DateTime(2015, 1, 1, 12, 0, 0, DateTimeKind.Utc);
                
                var notification = CreateNotification(currentTemp, coldSpellTemp, enteredAtUtc);

                await Handler.HandleAsync(notification);

                SmsClient.Received().Send(From, To, "12:00: Entered cold spell. (Temp: 1.5C).");
                ColdSpellRepository.Received().SaveAsync(Arg.Is<ColdSpell>(cs => cs.AlertedAt == _utcNow));
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

        public class Given_Alert_Recently_Sent : ColdSpellEnteredNotificationHandlerFixture
        {
            private readonly DateTime _lastAlertedAt = new DateTime(2015, 1, 1, 9, 0, 0, DateTimeKind.Utc);

            protected override void Before()
            {
                var coldSpell = new ColdSpell {AlertedAt = _lastAlertedAt};
                ColdSpellRepository.GetAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(coldSpell));
                DateTimeProvider.UtcNow().Returns(_lastAlertedAt.AddHours(6));
            }

            [Test]
            public async void When_Notifiction_Is_Handled_Then_Should_Not_Send_Sms()
            {
                const double currentTemp = 1.5;
                const double coldSpellTemp = 2.0;
                var enteredAtUtc = DateTime.UtcNow;

                var notification = CreateNotification(currentTemp, coldSpellTemp, enteredAtUtc);

                await Handler.HandleAsync(notification);

                SmsClient.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            }
        }
    }
}