using Newtonsoft.Json;

namespace Rumr.Plantduino.Worker.Messages
{
    public class SensorEvent : Message
    {
        public double Temperature { get; private set; }

        public double Lux { get; private set; }

        [JsonConstructor]
        public SensorEvent(double temp, double lux)
        {
            Temperature = temp;
            Lux = lux;
        }
    }
}