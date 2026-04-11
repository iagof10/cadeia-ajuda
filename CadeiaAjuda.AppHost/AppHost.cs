var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume("cadeia-ajuda-pgdata");

var database = postgres.AddDatabase("cadeiaajudadb");

var apiService = builder.AddProject<Projects.CadeiaAjuda_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.CadeiaAjuda_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
