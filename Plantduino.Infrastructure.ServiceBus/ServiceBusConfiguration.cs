using Microsoft.WindowsAzure;

namespace Rumr.Plantduino.Infrastructure.ServiceBus
{
    public class ServiceBusConfiguration : IServiceBusConfiguration
    {
        public string ConnectionString
        {
            get { return CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString"); }
        }
    }
}