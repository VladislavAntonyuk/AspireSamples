using System.Net.Http.Json;

namespace MauiAspireOllama;

public record WeatherForecast
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}

public class Response
{
	public WeatherForecast[] Forecast { get; set; } = [];
}