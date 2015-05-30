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
    }
}