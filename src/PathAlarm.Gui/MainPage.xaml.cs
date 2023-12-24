using NetMQ.Sockets;
using PathAlarm.Gui.Services;
using NetMQ;

namespace PathAlarm.Gui;

public partial class MainPage : ContentPage
{
    private IForegroundService fservice;
    private SubscriberSocket socket;
    private readonly IVibrationService vibrationService;
    private readonly CoordinateNotificationManager coordinateNotificationManager = new ();
    private readonly ILocalNotificationManager localNotificationManager;
    int count = 0;
    public MainPage(IForegroundService service, ILocalNotificationManager localNotificationManager, IVibrationService vibrationService)
    {
        this.vibrationService = vibrationService;
        fservice = service;
        this.localNotificationManager = localNotificationManager;
        InitializeComponent();
    }

    private async void OnServiceStartClicked(object sender, EventArgs e)
    {
        var s1 = await PermissionHelper.CheckAndRequestPermissionAsync<Permissions.LocationWhenInUse>();
        if (s1 != PermissionStatus.Granted)
        {
            return;
        }
        var status = await PermissionHelper.CheckAndRequestPermissionAsync<Permissions.LocationAlways>();
        if (status != PermissionStatus.Granted)
        {
            return;
        }
        editor.Text = "Started!\nHello";
        //fservice.SetAction(FserviceOnLocationUpdated);
        fservice.Start();
        socket = new SubscriberSocket();
        socket.Options.ReceiveHighWatermark = 1000;
        socket.Connect("tcp://localhost:13344");
        socket.Subscribe("TopicA");
        CheckLocation();
    }

    private void CheckLocation()
    {
        Task.Run(() =>
        {
            while (true)
            {
                socket.ReceiveFrameString();
                string messageReceived = socket.ReceiveFrameString();
                var call = coordinateNotificationManager.ShouldNotify(messageReceived, out var distance);
                if (call)
                {
                    vibrationService.Vibrate();
                    localNotificationManager.Show($"You are {distance} meter(s) away", "Path Alarm");
                    
                }
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    editor.Text = $"Coordinates are {messageReceived}\nDistance is {distance}";
                });
                Console.WriteLine(messageReceived);
                
            }
        });

    }
    private void FserviceOnLocationUpdated(double[] e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            editor.Text = $"Coordinates are {e[0]}; {e[1]}; {e[2]}";
        });
    }

    //method to stop manually foreground service
    private void Button_Clicked(object sender, EventArgs e)
    {
        fservice.Stop();
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        coordinateNotificationManager.Add(coordinateInput.Text);
        milestones.Text += coordinateInput.Text + "\n";
    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {
        coordinateNotificationManager.UpdateDistance(distanceInput.Text);
    }

    private void OnRemoveAlarm(object sender, EventArgs e)
    {
        coordinateNotificationManager.Remove();
    }
}