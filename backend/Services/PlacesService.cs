using System.Text.Json;
using NeighborhoodIntel.Api.Models;

namespace NeighborhoodIntel.Api.Services;

public class PlacesService(HttpClient http, IConfiguration config, ILogger<PlacesService> logger)
{
    private readonly string _apiKey = config["GoogleMaps:ApiKey"]
        ?? throw new InvalidOperationException("GoogleMaps:ApiKey not configured");

    private static readonly Dictionary<string, string[]> TypeMap = new()
    {
        ["schools"]     = ["school"],
        ["parks"]       = ["park"],
        ["grocery"]     = ["supermarket", "grocery_or_supermarket"],
        ["transit"]     = ["transit_station", "subway_station", "train_station", "bus_station", "light_rail_station"],
        ["restaurants"] = ["restaurant"],
    };

    public async Task<(PlaceCounts Counts, List<PlaceDetail> Places)> GetCountsAsync(
        double lat, double lng, int radiusMeters)
    {
        var categoryTasks = TypeMap.Select(kv =>
            FetchCategoryAsync(lat, lng, radiusMeters, kv.Key, kv.Value));

        var categoryResults = await Task.WhenAll(categoryTasks);

        var allPlaces = categoryResults.SelectMany(x => x).ToList();

        var counts = new PlaceCounts
        {
            Schools     = allPlaces.Count(p => p.Category == "schools"),
            Parks       = allPlaces.Count(p => p.Category == "parks"),
            Grocery     = allPlaces.Count(p => p.Category == "grocery"),
            Transit     = allPlaces.Count(p => p.Category == "transit"),
            Restaurants = allPlaces.Count(p => p.Category == "restaurants"),
        };

        return (counts, allPlaces);
    }

    private async Task<List<PlaceDetail>> FetchCategoryAsync(
        double lat, double lng, int radius, string category, string[] types)
    {
        // fetch all types, deduplicate by place_id
        var typeTasks = types.Select(type => FetchPlacesAsync(lat, lng, radius, type, category));
        var typeResults = await Task.WhenAll(typeTasks);

        return typeResults
            .SelectMany(x => x)
            .GroupBy(p => p.PlaceId)
            .Select(g => g.First())
            .ToList();
    }

    private async Task<List<PlaceDetail>> FetchPlacesAsync(
        double lat, double lng, int radius, string type, string category)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json" +
                  $"?location={lat},{lng}&radius={radius}&type={type}&key={_apiKey}";

        logger.LogInformation("→ Google Places: type={Type} radius={Radius}m", type, radius);
        var response = await http.GetStringAsync(url);
        var doc = JsonDocument.Parse(response);

        if (!doc.RootElement.TryGetProperty("results", out var results))
            return [];

        var places = results.EnumerateArray()
            .Select(p =>
            {
                var location = p.GetProperty("geometry").GetProperty("location");
                return new PlaceDetail
                {
                    PlaceId  = p.TryGetProperty("place_id", out var id)   ? id.GetString()   ?? "" : "",
                    Name     = p.TryGetProperty("name",     out var name) ? name.GetString() ?? "" : "",
                    Lat      = location.GetProperty("lat").GetDouble(),
                    Lng      = location.GetProperty("lng").GetDouble(),
                    Category = category,
                };
            })
            .Where(p => p.PlaceId.Length > 0)
            .ToList();

        logger.LogInformation("← Google Places: type={Type} → {Count} results", type, places.Count);
        return places;
    }
}
