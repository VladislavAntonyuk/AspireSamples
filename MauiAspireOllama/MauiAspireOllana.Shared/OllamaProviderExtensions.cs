namespace MauiAspireOllana.Shared;

using Microsoft.Extensions.DependencyInjection;
using Ollama;

public static class OllamaProviderExtensions
{
	public static IServiceCollection AddOllamaProvider(this IServiceCollection services, string baseUrl)
	{
		services.AddHttpClient<IOllamaApiClient, OllamaApiClient>(client =>
		        {
			        client.Timeout = TimeSpan.FromHours(24);
					client.BaseAddress = new Uri(baseUrl);
		        })
				.AddStandardResilienceHandler(x =>
				{
					x.AttemptTimeout.Timeout = TimeSpan.FromMinutes(10);
					x.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(30);
					x.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(30);
				});
		services.AddSingleton<OllamaProvider>();
		return services;
	}
}