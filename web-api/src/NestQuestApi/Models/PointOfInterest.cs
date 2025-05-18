using System.ComponentModel;

namespace NestQuestApi.Models;

public class PointOfInterest
{
    public double Latitude { get; }
    public double Longitude { get; }
    public Category Category { get; }

    public PointOfInterest(double lat, double lon)
    {
        Latitude = lat;
        Longitude = lon;
    }
}

public record Home(string DisplayName, double Lat, double Lon);

public enum Category
{
    [Description("Unknown")]
    Unknown,
    Library,
    School,
    Park,
    BikeTrail,
    Grocery,
    CoffeeShop,
    Airport,
    TrainStation,
    BusStation,
    PoliceStation,
    FireStation,
}