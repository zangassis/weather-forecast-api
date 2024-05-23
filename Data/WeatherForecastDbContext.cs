using Bogus;
using Microsoft.EntityFrameworkCore;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Data;

public class WeatherForecastDbContext : DbContext
{
    public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options)
        : base(options) { }

    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    public void SeedDatabase(int numberOfRecords)
    {
        if (WeatherForecasts.Any())
            return;

        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        var faker = new Faker<WeatherForecast>()
         .RuleFor(w => w.City, f => f.Address.City())
         .RuleFor(w => w.TemperatureC, f => f.Random.Double(-20, 55))
         .RuleFor(w => w.Summary, f => f.PickRandom(summaries))
         .RuleFor(w => w.Date, f => f.Date.Past(0));

        var weatherData = faker.Generate(numberOfRecords);

        WeatherForecasts.AddRange(weatherData);

        SaveChanges();
    }
}