namespace Rumr.Plantduino.Domain.Messages.Telemetry
{
    public class TemperatureTelemetry : TelemetryMessage
    {
        public double Temperature { get; set; }

        public static TemperatureTelemetry Create(int deviceId, double temp)
        {
            return new TemperatureTelemetry
            {
                DeviceId = deviceId,
                Temperature = temp
            };
        }
    }
}