namespace Rumr.Plantduino.Domain.Messages.Telemetry
{
    public class MoistureTelemetry : TelemetryMessage
    {
        public int Moisture { get; set; }
    }
}