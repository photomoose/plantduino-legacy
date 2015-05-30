using System;
using System.IO;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Rumr.Plantduino.Domain.Messages;

namespace Rumr.Plantduino.Infrastructure.ServiceBus
{
    public class MessageMapper
    {
        public static T Map<T>(BrokeredMessage message) where T : Message
        {
            var json = new StreamReader(message.GetBody<Stream>(), Encoding.UTF8).ReadToEnd();
            var entity = JsonConvert.DeserializeObject<T>(json);
            entity.DeviceId = (int)message.Properties["DeviceId"];
            entity.EnqueuedTimeUtc = message.EnqueuedTimeUtc;
            entity.CompletionTarget = message.CompleteAsync;

            return entity;
        }

        public static Message Map(BrokeredMessage message, Type messageType)
        {
            var json = new StreamReader(message.GetBody<Stream>(), Encoding.UTF8).ReadToEnd();
            var entity = (Message)JsonConvert.DeserializeObject(json, messageType);
            entity.DeviceId = (int)message.Properties["DeviceId"];
            entity.EnqueuedTimeUtc = message.EnqueuedTimeUtc;

            return entity;
        }

        public static BrokeredMessage Map<T>(T entity) where T : Message
        {
            var json = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity)));
            var brokeredMessage = new BrokeredMessage(json);
            brokeredMessage.Properties.Add("MessageType", entity.MessageType);
            brokeredMessage.Properties.Add("DeviceId", entity.DeviceId);

            return brokeredMessage;
        }
    }
}