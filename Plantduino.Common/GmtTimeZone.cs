using System;

namespace Rumr.Plantduino.Common
{
    public class GmtTimeZone : ITimeZone
    {
        private readonly TimeZoneInfo _timeZone;

        public GmtTimeZone()
        {
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");            
        }

        public DateTime ToLocalTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, _timeZone);            
        }
    }
}