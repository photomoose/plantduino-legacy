using System.Diagnostics;
using Rumr.Plantduino.Domain.Sms;

namespace Plantduino.Infrastructure.Twilio
{
    public class TraceSmsClient : ISmsClient
    {
        public void Send(string @from, string to, string text)
        {
            Trace.TraceInformation("Sent SMS to {0}: {1}", to, text);
        }
    }
}