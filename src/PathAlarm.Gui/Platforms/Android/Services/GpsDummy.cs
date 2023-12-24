using PathAlarm.Gui.Services;

namespace PathAlarm.Gui.Platforms.Android.Services;

public class GpsDummy : IGpsManager
{
    //home 52.38238998765395, 4.885555520161217
    //turn 52.38080445050047, 4.888348244852404
    private readonly double[] coordinates = new double[] { 52.38238998765395, 4.885555520161217, 10 };
    private readonly double[] dest = new double[] { 52.38080445050047, 4.888348244852404, 10 };
    private double[] delta = new double[3];
    private int currentStep = 0;

    public GpsDummy()
    {
        var steps = 5;
        var deltalat = (dest[0] - coordinates[0]) / steps;
        var deltalon = (dest[1] - coordinates[1]) / steps;
        delta = new double[] { deltalat, deltalon, 0 };
    }

    public Task<double[]> GetCurrentCoordinates()
    {
        if (currentStep >= 5)
        {
            return Task.FromResult(coordinates);
        }
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i] += delta[i];
        }

        currentStep++;
        return Task.FromResult(coordinates);
    }
}