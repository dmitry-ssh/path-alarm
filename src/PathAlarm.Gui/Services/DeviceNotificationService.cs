using PathAlarm.Engine.Notifications;

namespace PathAlarm.Gui.Services;

public class DeviceNotificationService : IDeviceNotificationService
{
    private readonly ILocalNotificationManager localNotificationManager;
    private readonly IVibrationService vibrationService;

    public DeviceNotificationService(ILocalNotificationManager localNotificationManager, IVibrationService vibrationService)
    {
        this.localNotificationManager = localNotificationManager;
        this.vibrationService = vibrationService;
    }

    public void NotifyUser(string message, string title)
    {
        vibrationService.Vibrate();
        localNotificationManager.Show(message, title);
    }
}