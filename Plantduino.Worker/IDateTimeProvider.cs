using System;

namespace Rumr.Plantduino.Worker
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}