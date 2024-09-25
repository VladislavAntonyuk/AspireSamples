namespace DistributedApplicationAuth.Web;

using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

/// <summary>
/// A DelegatingHandler implementation that add an authorization header with a token on behalf of the current user.
/// </summary>
public class MicrosoftIdentityUserAuthenticationMessageHandler(
	ITokenAcquisition tokenAcquisition,
	MicrosoftIdentityConsentAndConditionalAccessHandler handler,
	IOptionsMonitor<MicrosoftIdentityAuthenticationMessageHandlerOptions> namedMessageHandlerOptions,
	string? serviceName = null)
	: MicrosoftIdentityAuthenticationBaseMessageHandler(tokenAcquisition, namedMessageHandlerOptions, serviceName)
{
	private readonly IOptionsMonitor<MicrosoftIdentityAuthenticationMessageHandlerOptions> namedMessageHandlerOptions = namedMessageHandlerOptions;

	/// <inheritdoc/>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var authResult = await TokenAcquisition.GetAccessTokenForUserAsync(namedMessageHandlerOptions.CurrentValue.GetScopes())
		                                       .ConfigureAwait(false);


		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult);

		try
		{
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}

		return new HttpResponseMessage() { StatusCode = HttpStatusCode.Moved };
	}
}