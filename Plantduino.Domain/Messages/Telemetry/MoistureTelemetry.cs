namespace Rumr.Plantduino.Domain.Messages.Telemetry
{
    public class MoistureTelemetry : TelemetryMessage
    {
        public int Moisture { get; set; }

        public string SensorId { get; set; }

        public static MoistureTelemetry Create(string deviceId, string sensorId, int moisture)
        {
            return new MoistureTelemetry
            {
                DeviceId = deviceId,
                SensorId = sensorId,
                Moisture = moisture
            };
        }
    }
}