using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.Commands
{
    public class ColdLedOnCommand : Message
    {
        public double Temperature { get; set; }

        public ColdLedOnCommand(int deviceId, double temperature)
        {
            DeviceId = deviceId;
            Temperature = temperature;
        }
    }
}