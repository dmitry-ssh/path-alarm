using PathAlarm.Gui.Services;
using PathAlarm.Engine.Main;

namespace PathAlarm.Gui;

public partial class MainPage : ContentPage
{
    public MainPage(IForegroundService service, ILocalNotificationManager localNotificationManager, IVibrationService vibrationService,
        MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = mainViewModel;
    }
}