using MauiAspireOllama.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama", 11434);

var apiService = builder.AddProject<Projects.MauiAspireOllama_ApiService>("apiservice")
                        .WithReference(ollama);

builder.AddProject<Projects.MauiAspireOllama_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(apiService);

// MAUI projects are registered differently than other project types, with AddMobileProject. Aspire settings
// that are normally set as environment variables at launch time are in the case of MAUI instead generated
// in the specified MAUI app project directory, in the AspireAppSettings.g.cs file.
builder.AddMobileProject("mauiclient", "../MauiAspireOllama")
	.WithReference(apiService)
	.WithReference(ollama);

builder.Build().Run();
