namespace PathAlarm.Models.Coordinates;

public class GpsCoordinate
{
    public GpsCoordinate()
    {
    }

    public GpsCoordinate(double lat, double lon)
    {
        Lat = lat;
        Lon = lon;
    }

    public double Lat { get; }
    public double Lon { get; }
}