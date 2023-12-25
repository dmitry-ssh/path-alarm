using MVVMBase.Infrastructure;
using PathAlarm.Engine.Main;
using PathAlarm.Gui.Services;
using Plugin.LocalNotification;
using PathAlarm.Engine.Notifications;
using PathAlarm.Engine.Coordinates;
using PathAlarm.Engine.Permissions;
#if ANDROID
using PathAlarm.Gui.Platforms.Android.Services;
#endif

namespace PathAlarm.Gui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            Context.InvokeOnUiThread  = MainThread.BeginInvokeOnMainThread;
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if ANDROID
            builder.Services.AddSingleton<IForegroundService, ForegroundServiceDemo>();
            builder.Services.AddSingleton<ILocalNotificationManager, LocalNotificationManager>();
            builder.Services.AddSingleton<IVibrationService, VibrationService>();
            builder.Services.AddSingleton<IPermissionManager, PermissionManager>();
#endif
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>(); 
            builder.Services.AddSingleton<LocationChecker>();
            builder.Services.AddSingleton<CoordinateNotificationManager>();
            builder.Services.AddSingleton<IDeviceNotificationService, DeviceNotificationService>();
            builder.UseLocalNotification();
            return builder.Build();
        }
    }
}