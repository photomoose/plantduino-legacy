using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.Commands
{
    public class ColdLedOffCommand : Message
    {
        public double Temperature { get; set; }

        public ColdLedOffCommand(int deviceId, double temperature)
        {
            DeviceId = deviceId;
            Temperature = temperature;
        }
    }
}