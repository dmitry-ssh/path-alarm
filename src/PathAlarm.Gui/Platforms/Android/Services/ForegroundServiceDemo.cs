using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using PathAlarm.Engine.Coordinates;
using PathAlarm.Engine.Main;
using PathAlarm.Gui.Services;

namespace PathAlarm.Gui.Platforms.Android.Services;
[Service]
public class ForegroundServiceDemo : Service, IForegroundService
{
    private string NOTIFICATION_CHANNEL_ID = "10374";
    private int NOTIFICATION_ID = 1;
    private string NOTIFICATION_CHANNEL_NAME = "notification";
    private Timer timer;
    private readonly IGpsManager gpsManager = new GpsDummy();
    //private IGpsManager gpsManager = new GpsManager();
    private readonly LocationPublisher locationPublisher = new();

    public override IBinder OnBind(Intent intent)
    {
        throw new NotImplementedException();
    }

    [return: GeneratedEnum]//we catch the actions intents to know the state of the foreground service
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        if (intent.Action == "START_SERVICE")
        {
            locationPublisher.Start();
            timer = new Timer(Callback, this, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            RegisterNotification();
        }
        else if (intent.Action == "STOP_SERVICE")
        {
            timer.Dispose();
            locationPublisher.Dispose();
            StopForeground(true);
            StopSelfResult(startId);
        }

        return StartCommandResult.NotSticky;
    }

    private void Callback(object state)
    {
        Task.Run(async () =>
        {
            var coordinates = await gpsManager.GetCurrentCoordinates();
            var coords = $"{coordinates[0].ToString(CultureInfo.InvariantCulture)}, {coordinates[1].ToString(CultureInfo.InvariantCulture)}, {coordinates[2].ToString(CultureInfo.InvariantCulture)}";
            Console.WriteLine(coords);
            locationPublisher.Send(coords);
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            var notification = CreateNotification(coords);
            notificationManager?.Notify(NOTIFICATION_ID, notification.Build());
        });
        Console.WriteLine($"Callback!!!");
    }

    //Start and Stop Intents, set the actions for the MainActivity to get the state of the foreground service
    //Setting one action to start and one action to stop the foreground service
    public void Start()
    {
        Intent startService = new Intent(MainActivity.ActivityCurrent, typeof(ForegroundServiceDemo));
        startService.SetAction("START_SERVICE");
        MainActivity.ActivityCurrent.StartService(startService);
    }

    public void Stop()
    {
        Intent stopIntent = new Intent(MainActivity.ActivityCurrent, this.Class);
        stopIntent.SetAction("STOP_SERVICE");
        MainActivity.ActivityCurrent.StartService(stopIntent);
    }
    private void RegisterNotification()
    {
        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel(notificationManager);
        }

        var notification = CreateNotification("Foreground Service is running");
        StartForeground(NOTIFICATION_ID, notification.Build());
    }

    private NotificationCompat.Builder CreateNotification(string text)
    {
        var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
        notification.SetAutoCancel(false);
        notification.SetOngoing(true);
        notification.SetSmallIcon(Resource.Mipmap.appicon);
        notification.SetContentTitle("ForegroundService");
        notification.SetContentText(text);
        return notification;
    }

    private void CreateNotificationChannel(NotificationManager notificationMnaManager)
    {
        var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME,
            NotificationImportance.Low);
        notificationMnaManager.CreateNotificationChannel(channel);
    }
}