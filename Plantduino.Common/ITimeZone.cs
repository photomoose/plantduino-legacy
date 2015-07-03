using System;

namespace Rumr.Plantduino.Common
{
    public interface ITimeZone
    {
        DateTime ToLocalTime(DateTime dateTime);
    }
}