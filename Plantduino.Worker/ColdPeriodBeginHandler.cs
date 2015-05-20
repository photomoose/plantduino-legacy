using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public class ColdPeriodBeginHandler : ISystemEventHandler
    {
        private readonly ITopicSubscriber _topicSubscriber;

        public Filter SubscriptionFilter
        {
            get { return new SqlFilter(string.Format("EventType = '{0}'", Name)); }
        }

        public string Name
        {
            get { return EventTypes.ColdPeriodBegin; }
        }

        public ColdPeriodBeginHandler(ITopicSubscriber topicSubscriber)
        {
            _topicSubscriber = topicSubscriber;
        }

        public async Task ReceiveAsync()
        {
            var msg = await _topicSubscriber.ReceiveFromTopicAsync(TopicPaths.SystemEvents, Name);

            if (msg != null)
            {
                Trace.TraceInformation("Received ColdPeriodBegin message.");

                await msg.CompleteAsync();
            }
        }
    }
}