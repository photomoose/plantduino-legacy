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
            entity.DeviceId = (string)message.Properties["DeviceId"];
            entity.CompletionTarget = message.CompleteAsync;

            if (entity.Timestamp == DateTime.MinValue)
            {
                entity.Timestamp = message.EnqueuedTimeUtc;
            }

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