var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DistributedApplicationAuth_ApiService>("apiservice");

builder.AddProject<Projects.DistributedApplicationAuth_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(apiService);

builder.Build().Run();
