using System;
using Rumr.Plantduino.Domain.Messages.Telemetry;

namespace Rumr.Plantduino.Domain
{
    public class TemperatureTelemetryIndex : TelemetryIndex<TemperatureTelemetry>
    {
        public double Temperature { get; private set; }

        public TemperatureTelemetryIndex(double temperature, int deviceId, DateTime timestampUtc)
        {
            Temperature = temperature;
            DeviceId = deviceId;
            TimestampUtc = timestampUtc;
        }
    }
}