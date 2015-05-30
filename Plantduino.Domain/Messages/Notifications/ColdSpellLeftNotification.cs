using System;
using Newtonsoft.Json;

namespace Rumr.Plantduino.Domain.Messages.Notifications
{
    public class ColdSpellLeftNotification : NotificationMessage
    {
        public double CurrentTemp { get; private set; }
        public double ColdSpellTemp { get; private set; }
        public double MinTemp { get; private set; }
        public DateTime EnteredAtUtc { get; private set; }
        public DateTime LeftAtUtc { get; private set; }

        [JsonIgnore]
        public TimeSpan Duration
        {
            get { return LeftAtUtc - EnteredAtUtc; }
        }

        public ColdSpellLeftNotification(int deviceId, double currentTemp, double coldSpellTemp, double minTemp, DateTime enteredAtUtc, DateTime leftAtUtc)
        {
            CurrentTemp = currentTemp;
            ColdSpellTemp = coldSpellTemp;
            MinTemp = minTemp;
            EnteredAtUtc = enteredAtUtc;
            LeftAtUtc = leftAtUtc;
            DeviceId = deviceId;
        }
    }
}