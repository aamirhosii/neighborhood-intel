using NeighborhoodIntel.Api.Models;

namespace NeighborhoodIntel.Api.Services;

public static class ScoringService
{
    // Weights per amenity type
    private const double SchoolWeight     = 2.0;
    private const double ParkWeight       = 2.0;
    private const double GroceryWeight    = 1.5;
    private const double TransitWeight    = 2.0;
    private const double RestaurantWeight = 1.0;

    // Raw score is normalised to 100 assuming a "perfect" neighbourhood reference
    private const double MaxRaw = 60.0;

    public static (double Score, string Label) Calculate(PlaceCounts counts)
    {
        double raw =
            Math.Min(counts.Schools,     5) * SchoolWeight  +
            Math.Min(counts.Parks,       5) * ParkWeight     +
            Math.Min(counts.Grocery,     5) * GroceryWeight  +
            Math.Min(counts.Transit,     5) * TransitWeight  +
            Math.Min(counts.Restaurants, 5) * RestaurantWeight;

        double score = Math.Round(Math.Min(raw / MaxRaw * 100, 100), 1);

        string label = score switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Average",
            >= 20 => "Below Average",
            _     => "Poor",
        };

        return (score, label);
    }
}
