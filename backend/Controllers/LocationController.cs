using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using NeighborhoodIntel.Api.Models;
using NeighborhoodIntel.Api.Services;

namespace NeighborhoodIntel.Api.Controllers;

[ApiController]
[Route("api")]
public class LocationController(
    GeocodingService geocoding,
    PlacesService places,
    AiSummaryService ai,
    IHttpClientFactory httpFactory,
    IConfiguration config) : ControllerBase
{
    [HttpGet("autocomplete")]
    public async Task<ActionResult> Autocomplete([FromQuery] string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
            return Ok(new { predictions = Array.Empty<object>() });

        var apiKey  = config["GoogleMaps:ApiKey"];
        var encoded = Uri.EscapeDataString(input);
        var url     = $"https://maps.googleapis.com/maps/api/place/autocomplete/json" +
                      $"?input={encoded}&types=address&key={apiKey}";

        var http     = httpFactory.CreateClient();
        var response = await http.GetStringAsync(url);
        var doc      = JsonDocument.Parse(response);

        var status = doc.RootElement.TryGetProperty("status", out var s) ? s.GetString() : "unknown";
        if (status != "OK" && status != "ZERO_RESULTS")
            return StatusCode(500, new { error = $"Google Autocomplete API error: {status}" });

        var predictions = doc.RootElement
            .GetProperty("predictions")
            .EnumerateArray()
            .Select(p => new {
                description = p.GetProperty("description").GetString(),
                placeId     = p.GetProperty("place_id").GetString(),
            })
            .ToArray();

        return Ok(new { predictions });
    }


    [HttpPost("analyze-location")]
    public async Task<ActionResult<AnalyzeResponse>> Analyze([FromBody] AnalyzeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Address))
            return BadRequest(new { error = "Address is required." });

        if (request.RadiusMeters is < 100 or > 5000)
            request.RadiusMeters = 1000;

        try
        {
            var (lat, lng, formattedAddress) = await geocoding.GeocodeAsync(request.Address);
            var (counts, placeDetails) = await places.GetCountsAsync(lat, lng, request.RadiusMeters);
            var (score, label) = ScoringService.Calculate(counts);

            return Ok(new AnalyzeResponse
            {
                Address      = formattedAddress,
                Latitude     = lat,
                Longitude    = lng,
                RadiusMeters = request.RadiusMeters,
                Counts       = counts,
                Score        = score,
                ScoreLabel   = label,
                Places       = placeDetails,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("ai-summary")]
    public async Task<ActionResult<object>> AiSummary([FromBody] AiSummaryRequest request)
    {
        try
        {
            var summary = await ai.SummarizeAsync(request.Counts, request.Score, request.Address);
            return Ok(new { summary });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class AiSummaryRequest
{
    public string Address { get; set; } = string.Empty;
    public PlaceCounts Counts { get; set; } = new();
    public double Score { get; set; }
}
