using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.BookSwap_Api>("bookswap-api");
api.WithEnvironment("ConnectionStrings__Mongo", "mongodb://mongo:27017/BookSwapDB");

var aggregator = builder.AddProject<Projects.BookSwap_Aggregator>("bookswap-aggregator")
                        .WithReference(api);


builder.AddProject<Projects.BookSwap_GrpcServices>("bookswap-grpcservices");

var healthChecks = builder.AddProject<Projects.BookSwap_HealthChecks>("bookswap-healthchecks")
                          .WithReference(aggregator)
                          .WithExternalHttpEndpoints();

builder.Build().Run();
