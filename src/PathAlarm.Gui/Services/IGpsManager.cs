namespace PathAlarm.Gui.Services;

public interface IGpsManager
{
    Task<double[]> GetCurrentCoordinates();
}