using MauiAspireOllana.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOllamaProvider(builder.Configuration.GetConnectionString("ollama")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (OllamaProvider ollamaProvider) =>
{
	await ollamaProvider.PullModelAsync("llama3:latest");
	try
	{
		var forecasts = await ollamaProvider.GetResponse<Response>(
			"llama3",
			"""
				Generate random weather for the next 5 days.
				Return the JSON Array with the next properties: (DateOnly Date, int TemperatureC, string? Summary). Example:
				{   
					"forecast":[
						{
							"Date": "2024-09-27",
							"TemperatureC": 32,
							"Summary": "Hot"
						}
					]
				}
			""");
		return forecasts?.Forecast;
	}
	catch (Exception e)
	{
		Console.WriteLine(e);
		var forecast = Enumerable.Range(1, 5).Select(index =>
			new WeatherForecast()
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = summaries[Random.Shared.Next(summaries.Length)]
			})
			.ToArray();
		return forecast;
	}
});

app.MapDefaultEndpoints();

app.Run();

public class WeatherForecast
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