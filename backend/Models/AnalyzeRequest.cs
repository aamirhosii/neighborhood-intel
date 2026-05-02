namespace NeighborhoodIntel.Api.Models;

public class AnalyzeRequest
{
    public string Address { get; set; } = string.Empty;
    public int RadiusMeters { get; set; } = 1000;
}
