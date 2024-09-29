namespace MauiAspireOllana.Shared;

using Microsoft.Extensions.DependencyInjection;
using Ollama;

public static class OllamaProviderExtensions
{
	public static IServiceCollection AddOllamaProvider(this IServiceCollection services, string baseUrl)
	{
		services.AddHttpClient<IOllamaApiClient, OllamaApiClient>(client => client.BaseAddress = new Uri(baseUrl))
				.AddStandardResilienceHandler(x =>
				{
					x.AttemptTimeout.Timeout = TimeSpan.FromMinutes(5);
					x.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);
					x.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(10);
				});
		services.AddSingleton<OllamaProvider>();
		return services;
	}
}