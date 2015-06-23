using System;
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

        public int MoistureMin
        {
            get { return int.Parse(CloudConfigurationManager.GetSetting("MoistureMin")); }
        }

        public TimeSpan IrrigationDuration
        {
            get { return TimeSpan.Parse(CloudConfigurationManager.GetSetting("IrrigationDuration")); }
        }
    }
}