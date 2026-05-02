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

// Allow https://*.vercel.app for preview deploys when any configured origin is on vercel.app (unless CORS_ALLOW_VERCEL_PREVIEWS=false).
// Set CORS_ALLOW_VERCEL_PREVIEWS=true to force on; false to force off (production vercel only, exact origins).
var previewOverride = Environment.GetEnvironmentVariable("CORS_ALLOW_VERCEL_PREVIEWS");
bool allowVercelPreview;
if (string.Equals(previewOverride, "false", StringComparison.OrdinalIgnoreCase))
    allowVercelPreview = false;
else if (string.Equals(previewOverride, "true", StringComparison.OrdinalIgnoreCase))
    allowVercelPreview = true;
else
{
    allowVercelPreview = corsOrigins.Any(static o =>
        Uri.TryCreate(o, UriKind.Absolute, out var u)
        && u.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase));
}

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
