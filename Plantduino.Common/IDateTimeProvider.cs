using System;

namespace Rumr.Plantduino.Common
{
    public interface IDateTimeProvider
    {
        DateTime ToLocalTime(DateTime dateTime);
        DateTime UtcNow();
    }
}
