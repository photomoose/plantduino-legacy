using System;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Rumr.Plantduino.Worker
{
    public class MessageSerializer
    {
        public static T Map<T>(BrokeredMessage message) where T : Message
        {
            var type = Type.GetType(message.Properties["MessageType"].ToString());
            var body = message.GetBody<string>();
            var obj = (T)JsonConvert.DeserializeObject(body, type);

            obj.EnqueuedTimeUtc = message.EnqueuedTimeUtc;

            return obj;
        }

        public static BrokeredMessage Map(Message message)
        {
            var body = JsonConvert.SerializeObject(message);

            var brokeredMessage = new BrokeredMessage(body);
            brokeredMessage.Properties["MessageType"] = message.GetType().ToString();

            return brokeredMessage;
        }
    }
}