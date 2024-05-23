using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeatherForecastAPI.Data;
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

app.MapPost("/weather", async (WeatherForecast forecast, WeatherForecastDbContext db) =>
{
    db.WeatherForecasts.Add(forecast);

    await db.SaveChangesAsync();

    return Results.Created($"/weather/{forecast.Id}", forecast);
});

app.MapPut("/weather/{id}", async (int id, WeatherForecast updatedForecast, WeatherForecastDbContext db) =>
{
    var forecast = await db.WeatherForecasts.FindAsync(id);

    if (forecast == null)
        return Results.NotFound();

    forecast.City = updatedForecast.City;
    forecast.TemperatureC = updatedForecast.TemperatureC;
    forecast.Summary = updatedForecast.Summary;
    forecast.Date = updatedForecast.Date;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/weather/{id}", async (int id, WeatherForecastDbContext db) =>
{
    var forecast = await db.WeatherForecasts.FindAsync(id);

    if (forecast == null)
        return Results.NotFound();

    db.WeatherForecasts.Remove(forecast);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();

    db.Database.Migrate();

    db.SeedDatabase(10000);
}

app.Run();
