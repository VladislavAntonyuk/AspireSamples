using Microsoft.Extensions.Logging;

namespace MauiAspireOllama;

using MauiAspireOllana.Shared;
using Ollama;

public partial class MainPage : ContentPage
{
    readonly ILogger<MainPage> logger;
    private readonly OllamaProvider ollamaProvider;
    readonly CancellationTokenSource closingCts = new();

    public MainPage(ILogger<MainPage> logger, OllamaProvider ollamaProvider)
    {
        InitializeComponent();

        this.logger = logger;
        this.ollamaProvider = ollamaProvider;

        pbLoading.IsVisible = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        closingCts.Cancel();
    }

    async void OnButtonClick(object sender, EventArgs e)
    {
        btnLoad.IsEnabled = false;
        pbLoading.IsVisible = true;

        try
        {
            await ollamaProvider.PullModelAsync("llama3:latest");
			var weather = await ollamaProvider.GetResponse<Response>(
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
            dgWeather.ItemsSource = weather?.Forecast;
            dgWeather.IsVisible = true;
        }
        catch (TaskCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading weather");

            dgWeather.IsVisible = false;
            dgWeather.ItemsSource = null;

            await DisplayAlert(ex.Message, "Error", "Ok");
        }

        pbLoading.IsVisible = false;
        btnLoad.IsEnabled = true;
    }
}
