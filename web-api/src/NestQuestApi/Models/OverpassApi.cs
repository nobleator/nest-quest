namespace NestQuestApi.Models;

public class OverpassApiResponse
{
    public double Version { get; set; }
    public string Generator { get; set; }
    public Osm3S Osm3S { get; set; }
    public List<Element> Elements { get; set; }

    public OverpassApiResponse()
    {
        Elements = new List<Element>();
    }
}

public class Osm3S
{
    public DateTime TimestampOsmBase { get; set; }
    public string Copyright { get; set; }
}

public class Element
{
    public string Type { get; set; }
    public long Id { get; set; }
    public decimal Lat { get; set; }
    public decimal Lon { get; set; }
    public Dictionary<string, string> Tags { get; set; }

    public Element()
    {
        Tags = new Dictionary<string, string>();
    }
}
