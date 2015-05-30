namespace Rumr.Plantduino.Domain.Messages.Telemetry
{
    public class LuxTelemetry : TelemetryMessage
    {
        public double Lux { get; set; }
    }
}