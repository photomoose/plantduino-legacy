namespace Rumr.Plantduino.Domain.Messages.Telemetry
{
    public class TemperatureTelemetry : TelemetryMessage
    {
        public double Temperature { get; set; }
    }
}