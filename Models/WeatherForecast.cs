namespace WeatherForecastAPI.Models;

public class WeatherForecast
{
    public int Id { get; set; }
    public string City { get; set; }
    public double TemperatureC { get; set; }
    public string Summary { get; set; }
    public DateTime Date { get; set; }
}