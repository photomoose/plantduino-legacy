using Rumr.Plantduino.Worker.Messages;
using Rumr.Plantduino.Worker.Sms;

namespace Rumr.Plantduino.Worker.Handlers
{
    public class ColdPeriodHandler
    {
        private readonly ISmsClient _smsClient;

        public ColdPeriodHandler(ISmsClient smsClient)
        {
            _smsClient = smsClient;
        }

        public void Process(ColdPeriodEvent message)
        {
            _smsClient.Send("", "", "Entered cold period.");
        }
    }
}