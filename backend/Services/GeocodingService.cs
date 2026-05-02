using System.Text.Json;

namespace NeighborhoodIntel.Api.Services;

public class GeocodingService(HttpClient http, IConfiguration config)
{
    private readonly string _apiKey = config["GoogleMaps:ApiKey"]
        ?? throw new InvalidOperationException("GoogleMaps:ApiKey not configured");

    public async Task<(double Lat, double Lng, string FormattedAddress)> GeocodeAsync(string address)
    {
        var encoded = Uri.EscapeDataString(address);
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encoded}&key={_apiKey}";

        var response = await http.GetStringAsync(url);
        var doc = JsonDocument.Parse(response);
        var root = doc.RootElement;

        if (root.GetProperty("status").GetString() != "OK")
            throw new Exception($"Geocoding failed: {root.GetProperty("status").GetString()}");

        var location = root
            .GetProperty("results")[0]
            .GetProperty("geometry")
            .GetProperty("location");

        var formatted = root
            .GetProperty("results")[0]
            .GetProperty("formatted_address")
            .GetString() ?? address;

        return (
            location.GetProperty("lat").GetDouble(),
            location.GetProperty("lng").GetDouble(),
            formatted
        );
    }
}
