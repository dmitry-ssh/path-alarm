namespace PathAlarm.Gui.Services;

public interface IForegroundService
{
    void Start();
    void Stop();
    void SetAction(Action<double[]> action);
}