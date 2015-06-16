namespace Rumr.Plantduino.Domain.Messages.Telemetry
{
    public class TemperatureTelemetry : TelemetryMessage
    {
        public double Temperature { get; set; }

        public string SensorId { get; set; }

        public static TemperatureTelemetry Create(string deviceId, string sensorId, double temp)
        {
            return new TemperatureTelemetry
            {
                DeviceId = deviceId,
                SensorId = sensorId,
                Temperature = temp
            };
        }
    }
}