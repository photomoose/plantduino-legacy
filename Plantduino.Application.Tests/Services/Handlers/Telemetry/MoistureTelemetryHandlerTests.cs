using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Application.Services.Handlers.Telemetry;
using Rumr.Plantduino.Domain.Configuration;
using Rumr.Plantduino.Domain.Messages.Commands;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Tests.Services.Handlers.Telemetry
{
    [TestFixture]
    public class MoistureTelemetryHandlerTests
    {
        public abstract class MoistureTelemetryHandlerFixture
        {
            protected MoistureTelemetryHandler Handler;
            protected IConfiguration Configuration;
            protected ICommandService CommandService;
            protected string DeviceId = "1";
            protected string SensorId = "sensorId";

            [SetUp]
            public void SetUp()
            {
                Configuration = Substitute.For<IConfiguration>();
                CommandService = Substitute.For<ICommandService>();
                Handler = new MoistureTelemetryHandler(CommandService, Configuration);

                Before();
            }

            protected virtual void Before()
            {
            }

            protected void GivenTheMoistureMinIs(int moistureMin)
            {
                Configuration.MoistureMin.Returns(moistureMin);
            }

            protected void GivenTheIrrigationDurationIs(TimeSpan timeSpan)
            {
                Configuration.IrrigationDuration.Returns(timeSpan);
            }
        }

        public class Given_Soil_Is_Moist : MoistureTelemetryHandlerFixture
        {
            private CommandMessage _capturedCommand;

            protected override void Before()
            {
                CommandService.RaiseAsync(Arg.Do<CommandMessage>(c => _capturedCommand = c));
            }

            [Test]
            public async Task When_Moisture_Drops_Below_Min_Then_Should_Send_Irrigate_Command()
            {
                GivenTheMoistureMinIs(10);
                GivenTheIrrigationDurationIs(TimeSpan.FromMinutes(5));

                var telemetry = MoistureTelemetry.Create(DeviceId, SensorId, 10);

                await Handler.HandleAsync(telemetry);

                var command = (IrrigateCommand)_capturedCommand;
                command.DeviceId.Should().Be(DeviceId);
                command.SensorId.Should().Be(SensorId);
                command.Duration.Should().Be(300000);
            }

            [Test]
            public async Task When_Moisture_Remains_Above_Min_Then_Should_Not_Send_Irrigate_Command()
            {
                GivenTheMoistureMinIs(10);

                var telemetry = MoistureTelemetry.Create(DeviceId, SensorId, 11);

                await Handler.HandleAsync(telemetry);

                CommandService.DidNotReceive().RaiseAsync(Arg.Any<IrrigateCommand>());
            }
        }
    }
}