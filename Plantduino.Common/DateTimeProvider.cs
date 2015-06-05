using System;

namespace Rumr.Plantduino.Common
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private readonly TimeZoneInfo _timeZone;

        public DateTimeProvider()
        {
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        }

        public DateTime ToLocalTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, _timeZone);
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}