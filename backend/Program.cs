using NeighborhoodIntel.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Railway (and similar hosts) assign a dynamic PORT; bind explicitly so Kestrel listens publicly.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();
builder.Services.AddHttpClient<GeocodingService>();
builder.Services.AddHttpClient<PlacesService>();
builder.Services.AddScoped<AiSummaryService>();

var corsOrigins = new List<string>(
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:3000"]);
// Optional on Railway etc.: CORS_ALLOWED_ORIGINS=https://app.example.com,https://staging.example.com
var corsExtra = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
if (!string.IsNullOrWhiteSpace(corsExtra))
{
    foreach (var o in corsExtra.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        if (!corsOrigins.Contains(o))
            corsOrigins.Add(o);
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(corsOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.Run();
