using System;

namespace Rumr.Plantduino.Domain.Repositories
{
    public class ColdSpell
    {
        public string DeviceId { get; set; }
        public string SensorId { get; set; }
        public DateTime AlertedAt { get; set; }
    }
}