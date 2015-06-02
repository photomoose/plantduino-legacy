using Microsoft.WindowsAzure;
using Rumr.Plantduino.Domain.Configuration;

namespace Rumr.Plantduino.Application
{
    public class Configuration :  IConfiguration
    {
        public double ColdSpellTemp
        {
            get { return double.Parse(CloudConfigurationManager.GetSetting("ColdSpellTemp")); }
        }

        public string SmsFrom
        {
            get { return CloudConfigurationManager.GetSetting("SmsFrom"); }
        }

        public string SmsTo
        {
            get { return CloudConfigurationManager.GetSetting("SmsTo"); }
        }
    }
}