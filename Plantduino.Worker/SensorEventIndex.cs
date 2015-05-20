using Rumr.Plantduino.Worker.Messages;

namespace Rumr.Plantduino.Worker
{
    public class SensorEventIndex : ISensorEventIndex
    {
        private readonly IElasticSearchWrapper _elasticSearchWrapper;

        public SensorEventIndex(IElasticSearchWrapper elasticSearchWrapper)
        {
            _elasticSearchWrapper = elasticSearchWrapper;
        }

        public void Add(SensorEvent sensorEvent)
        {
            _elasticSearchWrapper.Index("plantduino", "plantduino", sensorEvent);            
        }
    }
}