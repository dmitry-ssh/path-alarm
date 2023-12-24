using PathAlarm.Gui.Services;

namespace PathAlarm.Gui.Platforms.Android.Services;

public class GpsManager : IGpsManager
{
    private CancellationTokenSource cts;
    public async Task<double[]> GetCurrentCoordinates()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var location = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLocationAsync(request, cts.Token);

            if (location != null)
            {
                //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                return new[] { location.Latitude, location.Longitude, location.Altitude ?? 0.0 };
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            Console.WriteLine(fnsEx);
            // Handle not supported on device exception
        }
        catch (FeatureNotEnabledException fneEx)
        {
            Console.WriteLine(fneEx);
            // Handle not enabled on device exception
        }
        catch (PermissionException pEx)
        {
            Console.WriteLine(pEx);
            // Handle permission exception
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // Unable to get location
        }

        return new double[3];
    }
}