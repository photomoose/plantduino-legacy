using System;

namespace Rumr.Plantduino.Domain.Messages.Notifications
{
    public class ColdSpellLeftNotification : NotificationMessage
    {
        public double CurrentTemp { get; private set; }
        public double ColdSpellTemp { get; private set; }
        public double MinTemp { get; private set; }
        public DateTime EnteredAt { get; private set; }
        public DateTime LeftAt { get; private set; }

        public TimeSpan Duration
        {
            get { return LeftAt - EnteredAt; }
        }

        public ColdSpellLeftNotification(int deviceId, double currentTemp, double coldSpellTemp, double minTemp, DateTime enteredAt, DateTime leftAt)
        {
            CurrentTemp = currentTemp;
            ColdSpellTemp = coldSpellTemp;
            MinTemp = minTemp;
            EnteredAt = enteredAt;
            LeftAt = leftAt;
            DeviceId = deviceId;
        }
    }
}