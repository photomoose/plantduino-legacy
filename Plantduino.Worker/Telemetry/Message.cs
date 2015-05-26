using System;
using Newtonsoft.Json;

namespace Rumr.Plantduino.Worker.Telemetry
{
    public abstract class Message
    {
        [JsonIgnore]
        public string MessageType
        {
            get { return GetType().Name; }
        }

        [JsonIgnore]
        public int DeviceId { get; set; }

        [JsonIgnore]
        public DateTime EnqueuedTimeUtc { get; set; }

    }
}