namespace PathAlarm.Engine.Notifications;

public interface IDeviceNotificationService
{
    void NotifyUser(string message, string title);
}