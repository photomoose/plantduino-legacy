using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rumr.Plantduino.Domain.Messages
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

        [JsonIgnore]
        public Func<Task> CompletionTarget { get; set; }

        public async Task CompleteAsync()
        {
            await CompletionTarget();
        }
    }
}