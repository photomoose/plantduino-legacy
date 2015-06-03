using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Telemetry;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Tests.Services.Handlers.Telemetry
{
    public class TemperatureTelemetryHandlerTests
    {
        public abstract class TemperatureTelemetryHandlerFixture
        {
            protected IConfiguration Configuration;
            protected INotificationService NotificationService;
            protected TemperatureTelemetryHandler Handler;
            protected int DeviceId = 1;

            [SetUp]
            protected void SetUp()
            {
                Configuration = Substitute.For<IConfiguration>();
                NotificationService = Substitute.For<INotificationService>();

                Handler = new TemperatureTelemetryHandler(Configuration, NotificationService);

                Before();
            }

            protected TemperatureTelemetry CreateTemperatureTelemetry(double temp)
            {
                var telemetry = TemperatureTelemetry.Create(DeviceId, temp);
                telemetry.Timestamp = new DateTime(2015, 1, 1, 0, 0, 0);

                return telemetry;
            }

            protected void GivenTheColdSpellTemperatureIs(double coldSpellTemp)
            {
                Configuration.ColdSpellTemp.Returns(coldSpellTemp);
            }


            public virtual void Before()
            {
            }
        }

        [TestFixture]
        public class Given_Inside_Cold_Spell : TemperatureTelemetryHandlerFixture
        {
            private const double ColdSpellTemp = 3.0;
            private readonly DateTime _enteredAtUtc = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public override void Before()
            {
                const double currentTemp = 2.9;

                GivenTheColdSpellTemperatureIs(ColdSpellTemp);
                var telemetry = CreateTemperatureTelemetry(currentTemp);
                telemetry.Timestamp = _enteredAtUtc;

                Handler.HandleAsync(telemetry).Wait();

                NotificationService.ClearReceivedCalls();
            }

            [Test]
            public async Task When_Cold_Spell_Continues_Then_Should_Not_Raise_Another_Notification()
            {
                var telemetry = CreateTemperatureTelemetry(1.0);

                await Handler.HandleAsync(telemetry);

                NotificationService.DidNotReceive().RaiseAsync(Arg.Any<ColdSpellEnteredNotification>());
            }

            [Test]
            public async Task When_Cold_Spell_Ends_Then_Should_Raise_Cold_Spell_Ended_Notification()
            {
                var endedAtUtc = new DateTime(2015, 1, 1, 9, 0, 0, DateTimeKind.Utc);

                var telemetry = CreateTemperatureTelemetry(3.1);
                telemetry.Timestamp = endedAtUtc;

                await Handler.HandleAsync(telemetry);

            }
        }

        [TestFixture]
        public class Given_Outside_Cold_Spell : TemperatureTelemetryHandlerFixture
        {
            private const double ColdSpellTemp = 3.0;
            private NotificationMessage _capturedNotification;

            public override void Before()
            {
                GivenTheColdSpellTemperatureIs(ColdSpellTemp);

                NotificationService.RaiseAsync(Arg.Do<NotificationMessage>(n => _capturedNotification = n));
            }

            [Test]
            public async Task When_Temp_Remains_Above_Cold_Spell_Temp_Then_Should_Not_Raise_Notification()
            {
                var telemetry = CreateTemperatureTelemetry(3.1);

                await Handler.HandleAsync(telemetry);

                NotificationService.DidNotReceive().RaiseAsync(Arg.Any<ColdSpellEnteredNotification>());
            }

            [Test]
            public async Task When_Cold_Spell_Begins_Then_Should_Raise_Cold_Spell_Entered_Notification()
            {
                var telemetry = CreateTemperatureTelemetry(ColdSpellTemp);

                await Handler.HandleAsync(telemetry);

                var notification = (ColdSpellEnteredNotification)_capturedNotification;
                notification.DeviceId.Should().Be(DeviceId);
                notification.ColdSpellTemp.Should().Be(ColdSpellTemp);
                notification.CurrentTemp.Should().Be(ColdSpellTemp);
                notification.EnteredAtUtc.Should().Be(telemetry.Timestamp);
            }
        }
    }
}