using Microsoft.WindowsAzure;

namespace Rumr.Plantduino.Worker
{
    public class Configuration : ITwilioAccount, IServiceBusConfiguration
    {
        public string AccountSid
        {
            get { return CloudConfigurationManager.GetSetting("TwilioAccountSid"); }
        }

        public string AuthToken
        {
            get { return CloudConfigurationManager.GetSetting("TwilioAuthToken"); }
        }

        public string ConnectionString
        {
            get { return CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString"); }
        }
    }
}