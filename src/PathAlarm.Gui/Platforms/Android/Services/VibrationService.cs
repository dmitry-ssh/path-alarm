using PathAlarm.Gui.Services;

namespace PathAlarm.Gui.Platforms.Android.Services;

public class VibrationService : IVibrationService
{
    public async void Vibrate()
    {
        Vibration.Default.Vibrate(500);
        await Task.Delay(700);
        Vibration.Default.Vibrate(300);
        await Task.Delay(500);
        Vibration.Default.Vibrate(500);
        await Task.Delay(700);
    }
}