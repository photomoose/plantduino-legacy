namespace Rumr.Plantduino.Domain.Messages.Commands
{
    public class IrrigateCommand : CommandMessage
    {
        public string SensorId { get; private set; }
        public double Duration { get; private set; }

        public IrrigateCommand(string deviceId, string sensorId, double duration)
        {
            DeviceId = deviceId;
            SensorId = sensorId;
            Duration = duration;
        }
    }
}