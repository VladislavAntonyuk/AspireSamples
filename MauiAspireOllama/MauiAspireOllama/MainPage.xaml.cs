using Microsoft.Extensions.Logging;

namespace MauiAspireOllama;

using LLama.Common;
using LLama;
using MauiAspireOllana.Shared;

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

		LoadingIndicator.IsVisible = false;
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		closingCts.Cancel();
	}

	async void AmINeedAnUmbrellaClick(object sender, EventArgs e)
	{
		var weather = WeatherCollectionView.ItemsSource?.Cast<WeatherForecast>().FirstOrDefault();
		if (weather == null)
		{
			await DisplayAlert("Weather not loaded", "Please load the weather first", "Ok");
			return;
		}

		AmINeedAnUmbrellaResult.Text = string.Empty;

		var file = await FilePicker.PickAsync();
		if (file == null)
		{
			return;
		}


		LoadingIndicator.IsVisible = true;
		var @params = new ModelParams(file.FullPath)
		{
			ContextSize = 512
		};

		using var weights = LLamaWeights.LoadFromFile(@params);
		var executor = new StatelessExecutor(weights, @params);
		var prompt = $"""
		              I have the next {weather}. 
		              You are a weather forecast expert, that can Choose one of two options and answer the question Do I need an umbrella?.
		              If you choose Option 1 than answer: Yes, you need an umbrella.
		              If you choose Option 2 than answer: No, you don't need an umbrella.
		              """;
		var result = executor.InferAsync(
			prompt,
			new InferenceParams()
			{
                MaxTokens = 50,
				AntiPrompts = ["umbrella"]
			});

		await foreach (var r in result)
		{
			AmINeedAnUmbrellaResult.Text += r;
		}

		LoadingIndicator.IsVisible = false;
		await DisplayAlert("Result", AmINeedAnUmbrellaResult.Text, "Ok");
	}

	async void LoadWebWeatherClick(object sender, EventArgs e)
	{
		LoadWebWeather.IsEnabled = false;
		LoadingIndicator.IsVisible = true;

		try
		{
			await ollamaProvider.PullModelAsync("llama3.2:latest");
			var weather = await ollamaProvider.GetResponse<Response>(
				"llama3.2",
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
			WeatherCollectionView.ItemsSource = weather?.Forecast;
			WeatherCollectionView.IsVisible = true;
		}
		catch (TaskCanceledException ex)
		{
			await DisplayAlert(ex.Message, "Error", "Ok");
			return;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error loading weather");

			WeatherCollectionView.IsVisible = false;
			WeatherCollectionView.ItemsSource = null;

			await DisplayAlert(ex.Message, "Error", "Ok");
		}

		LoadingIndicator.IsVisible = false;
		LoadWebWeather.IsEnabled = true;
	}
}
