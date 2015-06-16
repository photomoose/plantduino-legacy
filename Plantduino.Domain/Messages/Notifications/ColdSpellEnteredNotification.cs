using System;

namespace Rumr.Plantduino.Domain.Messages.Notifications
{
    public class ColdSpellEnteredNotification : NotificationMessage
    {
        public string SensorId { get; private set; }
        public double CurrentTemp { get; private set; }
        public double ColdSpellTemp { get; private set; }
        public DateTime EnteredAt { get; private set; }

        public ColdSpellEnteredNotification(string deviceId, string sensorId, double currentTemp, double coldSpellTemp, DateTime enteredAt)
        {
            SensorId = sensorId;
            CurrentTemp = currentTemp;
            ColdSpellTemp = coldSpellTemp;
            EnteredAt = enteredAt;
            DeviceId = deviceId;
        }
    }
}