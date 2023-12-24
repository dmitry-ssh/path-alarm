using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using NetMQ;
using NetMQ.Sockets;
using PathAlarm.Gui.Services;

namespace PathAlarm.Gui.Platforms.Android.Services;
[Service]
public class ForegroundServiceDemo : Service, IForegroundService
{
    private string NOTIFICATION_CHANNEL_ID = "10374";
    private int NOTIFICATION_ID = 1;
    private string NOTIFICATION_CHANNEL_NAME = "notification";
    private Timer timer;
    private IGpsManager gpsManager = new GpsDummy();
    //private IGpsManager gpsManager = new GpsManager();
    private PublisherSocket publisherSocket;

    public override IBinder OnBind(Intent intent)
    {
        throw new NotImplementedException();
    }

    [return: GeneratedEnum]//we catch the actions intents to know the state of the foreground service
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        if (intent.Action == "START_SERVICE")
        {
            publisherSocket = new PublisherSocket();
            publisherSocket.Options.SendHighWatermark = 1000;
            publisherSocket.Bind("tcp://*:13344");
            timer = new Timer(Callback, this, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            RegisterNotification();//Proceed to notify
        }
        else if (intent.Action == "STOP_SERVICE")
        {
            timer.Dispose();
            publisherSocket.Dispose();
            StopForeground(true);//Stop the service
            StopSelfResult(startId);
        }

        return StartCommandResult.NotSticky;
    }

    private void Callback(object state)
    {
        var t = Task.Run(async () =>
        {
            var coordinates = await gpsManager.GetCurrentCoordinates();
            var coords = $"{coordinates[0].ToString(CultureInfo.InvariantCulture)}, {coordinates[1].ToString(CultureInfo.InvariantCulture)}, {coordinates[2].ToString(CultureInfo.InvariantCulture)}";
            Console.WriteLine(coords);
            publisherSocket?.SendMoreFrame("TopicA").SendFrame(coords);
            var notifcationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            var notification = CreateNotification(coords);
            notifcationManager.Notify(NOTIFICATION_ID, notification.Build());
            //LocationUpdated?.Invoke(coordinates);
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

    public void SetAction(Action<double[]> action)
    {
        LocationUpdated = action;
    }

    private Action<double[]> LocationUpdated { get; set; }

    private void RegisterNotification()
    {
        var notifcationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel(notifcationManager);
        }

        var notification = CreateNotification("Foreground Service is running");
        StartForeground(NOTIFICATION_ID, notification.Build());
        //NotificationChannel channel = new NotificationChannel("ServiceChannel", "ServiceDemo", NotificationImportance.Max);
        //NotificationManager manager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(Context.NotificationService);
        //manager.CreateNotificationChannel(channel);
        //Notification notification = new Notification.Builder(this, "ServiceChannel")
        //    .SetContentTitle("Service Working")
        //    .SetSmallIcon(Resource.Drawable.abc_ab_share_pack_mtrl_alpha)
        //    .SetOngoing(true)
        //    .Build();

        //StartForeground(100, notification);
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

    private void RequestLocation()
    {

    }
}