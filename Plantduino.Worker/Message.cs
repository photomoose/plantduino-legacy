using System;

namespace Rumr.Plantduino.Worker
{
    public abstract class Message
    {
        public DateTime EnqueuedTimeUtc { get; set; }
    }
}