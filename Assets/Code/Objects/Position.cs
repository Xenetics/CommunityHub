using System;

public class Position
{
    public double Latitude = 0;
    public double Longitude = 0;

    public Position(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    public static bool operator ==(Position a, Position b)
    {
        return (a.Latitude == b.Latitude) && (a.Longitude == b.Longitude);
    }

    public static bool operator !=(Position a, Position b)
    {
        return (a.Latitude != b.Latitude) || (a.Longitude != b.Longitude);
    }
}
