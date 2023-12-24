namespace PathAlarm.Gui.Models;

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

    public double Lat { get; set; }
    public double Lon { get; set; }
}