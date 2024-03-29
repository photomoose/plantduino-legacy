﻿using Microsoft.WindowsAzure;

namespace Plantduino.Infrastructure.Twilio
{
    public class TwilioAccount : ITwilioAccount
    {
        public string AccountSid
        {
            get { return CloudConfigurationManager.GetSetting("TwilioAccountSid"); }
        }

        public string AuthToken
        {
            get { return CloudConfigurationManager.GetSetting("TwilioAuthToken"); }
        }

        public bool IsSmsEnabled
        {
            get { return bool.Parse(CloudConfigurationManager.GetSetting("SmsEnabled")); }
        }
    }
}