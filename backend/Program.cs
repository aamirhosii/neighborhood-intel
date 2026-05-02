using System.Collections.Generic;
using System.Linq;
using NeighborhoodIntel.Api.Services;

static string NormalizeOrigin(string o) =>
    string.IsNullOrWhiteSpace(o) ? string.Empty : o.Trim().TrimEnd('/');

var builder = WebApplication.CreateBuilder(args);

// Railway (and similar hosts) assign a dynamic PORT; bind explicitly so Kestrel listens publicly.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
builder.Services.AddHttpClient<GeocodingService>();
builder.Services.AddHttpClient<PlacesService>();
builder.Services.AddScoped<AiSummaryService>();

var corsOrigins = new List<string>(
    (builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
     ?? ["http://localhost:5173", "http://localhost:3000"])
    .Select(NormalizeOrigin)
    .Where(s => s.Length > 0));
// Optional on Railway etc.: CORS_ALLOWED_ORIGINS=https://app.example.com,https://staging.example.com
var corsExtra = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
if (!string.IsNullOrWhiteSpace(corsExtra))
{
    foreach (var o in corsExtra.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        var n = NormalizeOrigin(o);
        if (n.Length > 0 && !corsOrigins.Contains(n))
            corsOrigins.Add(n);
    }
}

// Set CORS_ALLOW_VERCEL_PREVIEWS=true on Railway to allow any https://*.vercel.app origin (preview deploys).
var allowVercelPreview = string.Equals(
    Environment.GetEnvironmentVariable("CORS_ALLOW_VERCEL_PREVIEWS"),
    "true",
    StringComparison.OrdinalIgnoreCase);

var allowedExact = new HashSet<string>(corsOrigins, StringComparer.Ordinal);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;
                var trimmed = origin.TrimEnd('/');
                if (allowedExact.Contains(trimmed) || allowedExact.Contains(origin.Trim()))
                    return true;
                if (!allowVercelPreview) return false;
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                return uri.Scheme == Uri.UriSchemeHttps
                       && uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase);
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.Run();
