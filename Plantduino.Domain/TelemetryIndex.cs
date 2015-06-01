using System;
using Rumr.Plantduino.Domain.Messages.Telemetry;

namespace Rumr.Plantduino.Domain
{
    public abstract class TelemetryIndex<T> where T : TelemetryMessage
    {
        public DateTime TimestampUtc { get; protected set; }
        public int DeviceId { get; protected set; }

        public string MessageType
        {
            get { return typeof (T).Name; }
        }
    }
}