using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker
{
    public class MessageMapper
    {
        public static T Map<T>(BrokeredMessage message) where T : Message
        {
            var json = new StreamReader(message.GetBody<Stream>(), Encoding.UTF8).ReadToEnd();
            var entity = JsonConvert.DeserializeObject<T>(json);
            entity.DeviceId = (int)message.Properties["DeviceId"];
            entity.EnqueuedTimeUtc = message.EnqueuedTimeUtc;

            return entity;
        }

        public static BrokeredMessage Map<T>(T entity) where T : Message
        {
            var brokeredMessage = new BrokeredMessage(JsonConvert.SerializeObject(entity));
            brokeredMessage.Properties.Add("MessageType", entity.MessageType);
            brokeredMessage.Properties.Add("DeviceId", entity.DeviceId);

            return brokeredMessage;
        }
    }
}