namespace Rumr.Plantduino.Worker.Handlers
{
    public class ColdPeriodBegin : Message
    {
        public double Temperature { get; private set; }

        public ColdPeriodBegin(double temperature)
        {
            Temperature = temperature;
        }
    }
}