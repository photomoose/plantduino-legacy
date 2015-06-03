﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Telemetry;
using Rumr.Plantduino.Domain;
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
            protected IIndexService IndexClient;
            protected TemperatureTelemetryHandler Handler;
            protected int DeviceId = 1;

            [SetUp]
            protected void SetUp()
            {
                Configuration = Substitute.For<IConfiguration>();
                NotificationService = Substitute.For<INotificationService>();
                IndexClient = Substitute.For<IIndexService>();

                Handler = new TemperatureTelemetryHandler(Configuration, NotificationService, IndexClient);

                Before();
            }

            protected TemperatureTelemetry CreateTemperatureTelemetry(double temp)
            {
                var telemetry = TemperatureTelemetry.Create(DeviceId, temp);
                telemetry.EnqueuedTimeUtc = new DateTime(2015, 1, 1, 0, 0, 0);

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
            private NotificationMessage _capturedNotification;

            public override void Before()
            {
                const double currentTemp = 2.9;

                GivenTheColdSpellTemperatureIs(ColdSpellTemp);
                var telemetry = CreateTemperatureTelemetry(currentTemp);
                telemetry.EnqueuedTimeUtc = _enteredAtUtc;

                Handler.HandleAsync(telemetry).Wait();

                NotificationService.RaiseAsync(Arg.Do<NotificationMessage>(n => _capturedNotification = n));
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
                telemetry.EnqueuedTimeUtc = endedAtUtc;

                await Handler.HandleAsync(telemetry);

                var notification = (ColdSpellLeftNotification)_capturedNotification;
                notification.DeviceId.Should().Be(DeviceId);
                notification.ColdSpellTemp.Should().Be(ColdSpellTemp);
                notification.CurrentTemp.Should().Be(3.1);
                notification.EnteredAtUtc.Should().Be(_enteredAtUtc);
                notification.LeftAtUtc.Should().Be(endedAtUtc);
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
                notification.EnteredAtUtc.Should().Be(telemetry.EnqueuedTimeUtc);
            }
        }

        [TestFixture]
        public class Given_Any_Scenario : TemperatureTelemetryHandlerFixture
        {
            private TelemetryIndex<TemperatureTelemetry> _capturedIndex;

            public override void Before()
            {
                IndexClient.IndexAsync(Arg.Do<TelemetryIndex<TemperatureTelemetry>>(i => _capturedIndex = i));
            }

            [Test]
            public async Task When_Telemetry_Is_Received_Then_Telemetry_Should_Be_Indexed()
            {
                const double temperature = 10.0;

                var telemetry = CreateTemperatureTelemetry(temperature);

                await Handler.HandleAsync(telemetry);

                var index = (TemperatureTelemetryIndex) _capturedIndex;
                index.DeviceId.Should().Be(DeviceId);
                index.Temperature.Should().Be(temperature);
                index.TimestampUtc.Should().Be(telemetry.EnqueuedTimeUtc);
            }
        }
    }
}