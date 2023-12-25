using PathAlarm.Engine.Shared;
using PathAlarm.Models.Coordinates;

namespace PathAlarm.Engine.Coordinates;

public class CoordinatePointViewModel : ViewModel
{
    private GpsCoordinate gpsCoordinate;

    public CoordinatePointViewModel(GpsCoordinate coordinate)
    {
        GpsCoordinate = coordinate;
    }

    public GpsCoordinate GpsCoordinate
    {
        get => gpsCoordinate;
        set
        {
            if (Equals(value, gpsCoordinate))
            {
                return;
            }
            gpsCoordinate = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Text));
        }
    }

    public string Text => $"{GpsCoordinate.Lat:f6}; {GpsCoordinate.Lon:f6}";
}