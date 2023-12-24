using Geolocation;
using PathAlarm.Gui.Models;

namespace PathAlarm.Gui.Services;

public static class GpsGeometry
{
    public static double Distance(GpsCoordinate c1, GpsCoordinate c2)
    {
        var hc1 = new Coordinate(c1.Lat, c1.Lon);
        var hc2 = new Coordinate(c2.Lat, c2.Lon);
        var distance = GeoCalculator.GetDistance(hc1, hc2, 1, DistanceUnit.Meters);
        return distance;
    }
}