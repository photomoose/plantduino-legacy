using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rumr.Plantduino.Worker.Messages;

namespace Rumr.Plantduino.Worker.Handlers
{
    public class SensorEventHandler
    {
        private readonly ISensorEventIndex _index;
        private readonly ITopicPublisher _topicPublisher;
        private bool _isColdPeriod;

        public SensorEventHandler(ISensorEventIndex index, ITopicPublisher topicPublisher)
        {
            _index = index;
            _topicPublisher = topicPublisher;
        }

        public async Task ProcessAsync(SensorEvent message)
        {
            if (message.Temperature <= 2 && !_isColdPeriod)
            {
                _isColdPeriod = true;
                await _topicPublisher.SendToTopicAsync(TopicPaths.SystemEvents, new ColdPeriodBegin(message.Temperature));
            }
            else if (_isColdPeriod)
            {
                _isColdPeriod = false;
                await _topicPublisher.SendToTopicAsync(TopicPaths.SystemEvents, new ColdPeriodEnd());
            }

            _index.Add(message);
        }
    }

    public class ColdPeriodEnd : Message
    {
        
    }
}