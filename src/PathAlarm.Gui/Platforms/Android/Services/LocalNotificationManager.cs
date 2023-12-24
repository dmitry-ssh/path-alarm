using PathAlarm.Gui.Services;
using Plugin.LocalNotification;

namespace PathAlarm.Gui.Platforms.Android.Services;

internal class LocalNotificationManager : ILocalNotificationManager
{
    //public void Show(string message, string title)
    //{
    //}
    public async void Show(string message, string title)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        var notification = new NotificationRequest
        {
            NotificationId = 100,
            Title = title,
            Description = message,
            ReturningData = "Dummy data", // Returning data when tapped on notification.
            //Schedule =
            //{
            //    NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification, if not specified notification will show immediately.
            //}
        };
        await LocalNotificationCenter.Current.Show(notification);
    }
}