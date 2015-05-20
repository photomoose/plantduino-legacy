using Rumr.Plantduino.Worker.Messages;

namespace Rumr.Plantduino.Worker
{
    public interface ISensorEventIndex
    {
        void Add(SensorEvent sensorEvent);
    }
}