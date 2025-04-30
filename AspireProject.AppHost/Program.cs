var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithContainerName("Aspire-SQL")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("aspire-dbVolume");

var db = sql.AddDatabase("database");

var serviceBus = builder.AddAzureServiceBus("serviceBus")
    .RunAsEmulator()
    .AddServiceBusQueue("Test-queue");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("Aspire-Blobs");

var api = builder.AddProject<Projects.API>("api")
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WaitFor(db)
    .WithReference(serviceBus)
    .WaitFor(serviceBus);

builder.AddNpmApp("reactvite", "../frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddProject<Projects.Functions>("functions")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(serviceBus)
    .WaitFor(serviceBus)
    .WithReference(storage)
    .WaitFor(storage);


builder.AddProject<Projects.MigrationService>("migrationservice");


builder.Build().Run();
