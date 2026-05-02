namespace NeighborhoodIntel.Api.Models;

public class AnalyzeResponse
{
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RadiusMeters { get; set; }
    public PlaceCounts Counts { get; set; } = new();
    public double Score { get; set; }
    public string ScoreLabel { get; set; } = string.Empty;
    public string? AiSummary { get; set; }
    public List<PlaceDetail> Places { get; set; } = [];
}

public class PlaceCounts
{
    public int Schools { get; set; }
    public int Parks { get; set; }
    public int Grocery { get; set; }
    public int Transit { get; set; }
    public int Restaurants { get; set; }
}

public class PlaceDetail
{
    public string PlaceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Category { get; set; } = string.Empty;
}
