using System.Diagnostics;
using System.Globalization;
using PathAlarm.Gui.Models;

namespace PathAlarm.Gui.Services;

public class CoordinateNotificationManager
{
    private List<GpsCoordinate> coordinateList = new List<GpsCoordinate>();
    private int notifyDistance = 20;
    private GpsCoordinate currentCoordinate;
    public void Add(string coordinateString)
    {
        if (!ExtractCoordinate(coordinateString, out var coordinate))
        {
            return;
        }
        coordinateList.Add(coordinate);
    }

    public void Remove()
    {
        if (currentCoordinate == null)
        {
            return;
        }
        coordinateList.Remove(currentCoordinate);
    }

    public void UpdateDistance(string newInput)
    {
        if (!int.TryParse(newInput, out var newDistance))
        {
            return;
        }

        notifyDistance = newDistance;
    }
    private static bool ExtractCoordinate(string coordinateString, out GpsCoordinate coordinate)
    {
        coordinate = new GpsCoordinate();
        var parts = coordinateString.Split(", ", StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            return false;
        }

        var r1 = double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat);
        var r2 = double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon);
        if (!r1 || !r2)
        {
            return false;
        }

        coordinate = new GpsCoordinate(lat, lon);
        return true;
    }

    public bool ShouldNotify(string coordinateString, out double distance)
    {
        currentCoordinate = null;
        distance = Double.NaN;
        if (!ExtractCoordinate(coordinateString, out var coordinate))
        {
            return false;
        }

        foreach (var gpsCoordinate in coordinateList)
        {
            var dist = GpsGeometry.Distance(gpsCoordinate, coordinate);
            distance = dist;
            if (dist < notifyDistance)
            {
                currentCoordinate = gpsCoordinate;
                return true;
            }
        }
        return false;
    }
}