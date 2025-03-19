public class PointOfInterest
{
    public double Latitude { get; }
    public double Longitude { get; }
    public POIType Type { get; }

    public PointOfInterest(double lat, double lon)
    {
        Latitude = lat;
        Longitude = lon;
    }
}

public enum POIType
{
    Unknown,
    Library,
    School,
    Park,
}