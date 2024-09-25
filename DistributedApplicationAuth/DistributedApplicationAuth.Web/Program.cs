using DistributedApplicationAuth.Web;
using DistributedApplicationAuth.Web.Components;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MicrosoftIdentityUserAuthenticationMessageHandler = DistributedApplicationAuth.Web.MicrosoftIdentityUserAuthenticationMessageHandler;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

var scopes = builder.Configuration.GetSection("ApiClientDownstream:Scopes").Get<string[]>();
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAdB2C)
	.EnableTokenAcquisitionToCallDownstreamApi(scopes)
	.AddDownstreamApi("ApiClient", builder.Configuration.GetSection("ApiClientDownstream"))
	.AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddScoped<WeatherApiDownstreamApiClient>();

builder.Services.AddOptions<MicrosoftIdentityAuthenticationMessageHandlerOptions>()
        .Bind(builder.Configuration.GetSection("ApiClient"));
builder.Services.AddTransient<MicrosoftIdentityUserAuthenticationMessageHandler>();
builder.Services.AddHttpClient<WeatherApiHttpClient>(o=>
{
	var baseUrl = builder.Configuration.GetValue<string>("ApiClient:BaseUrl");
	o.BaseAddress = new Uri(baseUrl!);
})
.AddHttpMessageHandler<MicrosoftIdentityUserAuthenticationMessageHandler>();

builder.Services.AddOutputCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
