﻿using PathAlarm.Gui.Services;
using Plugin.LocalNotification;
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
#endif
            builder.Services.AddSingleton<MainPage>();
            builder.UseLocalNotification();
            return builder.Build();
        }
    }
}