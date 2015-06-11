using System;

namespace Rumr.Plantduino.Domain.Messages.Notifications
{
    public class ColdSpellEnteredNotification : NotificationMessage
    {
        public double CurrentTemp { get; private set; }
        public double ColdSpellTemp { get; private set; }
        public DateTime EnteredAt { get; private set; }

        public ColdSpellEnteredNotification(int deviceId, double currentTemp, double coldSpellTemp, DateTime enteredAt)
        {
            CurrentTemp = currentTemp;
            ColdSpellTemp = coldSpellTemp;
            EnteredAt = enteredAt;
            DeviceId = deviceId;
        }
    }
}