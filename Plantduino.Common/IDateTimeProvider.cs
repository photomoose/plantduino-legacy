using System;

namespace Rumr.Plantduino.Common
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}
