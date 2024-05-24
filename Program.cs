using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeatherForecastAPI.Data;
using WeatherForecastAPI.Extension;
using WeatherForecastAPI.Models;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGet("/weather/{city}", async (string city, WeatherForecastDbContext db, IMemoryCache cache) =>
{
    WeatherForecast? weather = null;

    if (!cache.TryGetValue(city, out weather))
    {
        weather = await db.WeatherForecasts
                          .OrderByDescending(w => w.Date)
                          .FirstOrDefaultAsync(w => w.City == city);

        if (weather != null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            cache.Set(city, weather, cacheEntryOptions);
        }
    }

    return weather == null ? Results.NotFound("Weather data not found for the specified city.") : Results.Ok(weather);
});

app.MapGet("/weather/cities", async (WeatherForecastDbContext db, IMemoryCache cache) =>
{
    const string cacheKey = "cities_with_weather";

    if (!cache.TryGetValue(cacheKey, out List<string> cities))
    {
        cities = await db.WeatherForecasts
                         .Select(w => w.City)
                         .Distinct()
                         .ToListAsync();

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        cache.Set(cacheKey, cities, cacheEntryOptions);
    }

    return Results.Ok(cities);
});

app.MapPost("/weather/cache-update/{city}", async (string city, WeatherForecastDbContext db, IMemoryCache cache) =>
{
    var weather = await db.WeatherForecasts
                          .OrderByDescending(w => w.Date)
                          .FirstOrDefaultAsync(w => w.City == city);

    if (weather != null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        cache.Set(city, weather, cacheEntryOptions);
        return Results.Ok($"Cache for city {city} updated.");
    }
    else
        return Results.NotFound("Weather data not found for the specified city.");
});

app.MapDelete("/weather/cache-delete/{city}", (string city, IMemoryCache cache) =>
{
    cache.Remove(city);
    return Results.Ok($"Cache for city {city} removed.");
});

app.MapDelete("/cache/clear", (IMemoryCache cache) =>
{
    if (cache is MemoryCache concreteMemoryCache)
        concreteMemoryCache.Clear();

    return Results.Ok("Cache cleared.");
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();

    db.Database.Migrate();

    db.SeedDatabase(10000);
}

app.Run();
