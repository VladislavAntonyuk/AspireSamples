var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DashboardConfiguration_ApiService>("apiservice");

builder.AddProject<Projects.DashboardConfiguration_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(apiService);

builder.Build().Run();
