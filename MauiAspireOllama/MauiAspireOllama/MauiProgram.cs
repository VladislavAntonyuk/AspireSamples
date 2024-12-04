using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MauiAspireOllama;

using MauiAspireOllana.Shared;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
		var mauiAppBuilder = MauiApp.CreateBuilder();

        mauiAppBuilder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        mauiAppBuilder.Configuration.AddInMemoryCollection(AspireAppSettings.Settings);
#endif

        mauiAppBuilder.AddAppDefaults();

#if DEBUG
        mauiAppBuilder.Logging.AddDebug();
#endif

        mauiAppBuilder.Services.AddOllamaProvider("http://localhost:11434/api");
        mauiAppBuilder.Services.AddSingleton<MainPage>();

        MauiApp mauiApp = mauiAppBuilder.Build();
        mauiApp.InitOpenTelemetryServices();
        return mauiApp;
    }
}
