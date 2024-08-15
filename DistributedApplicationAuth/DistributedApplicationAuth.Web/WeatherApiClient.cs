using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace DistributedApplicationAuth.Web;

public class WeatherApiClient(IDownstreamApi downstreamApi, MicrosoftIdentityConsentAndConditionalAccessHandler handler)
{
    public async Task<List<WeatherForecast>> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

		try
		{
			forecasts = await downstreamApi.GetForUserAsync<List<WeatherForecast>>("ApiClient", options =>
			{
				options.RelativePath = "/weatherforecast";
			}, cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}

		return forecasts ?? [];
    }
    
    public async Task<IdentityInfo?> GetMeAsync(CancellationToken cancellationToken = default)
    {
	    try
	    {
		    return await downstreamApi.GetForUserAsync<IdentityInfo>("ApiClient", options =>
		    {
			    options.RelativePath = "/me";
		    }, cancellationToken: cancellationToken);
	    }
	    catch (Exception e)
	    {
		    handler.HandleException(e);
	    }

	    return null;
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class IdentityInfo
{
	public string? Name { get; set; }
	public string? AuthenticationType { get; set; }
	public bool IsAuthenticated { get; set; }
}
