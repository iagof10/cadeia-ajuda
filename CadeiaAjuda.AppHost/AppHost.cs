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
    .WithHttpEndpoint(port: 5012, targetPort: 5012, name: "external-http", isProxied: false)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
