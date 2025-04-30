var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("serviceBus")
    .RunAsEmulator()
    .AddServiceBusQueue("Test-queue");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("Aspire-Blobs");

var sql = builder.AddSqlServer("sql")
    .WithContainerName("Aspire-SQL")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("aspire-dbVolume");

var db = sql.AddDatabase("database");

var migrations = builder.AddProject<Projects.MigrationService>("migrationservice")
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.API>("api")
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WaitFor(migrations)
    .WithReference(serviceBus);

builder.AddNpmApp("reactvite", "../frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddProject<Projects.Functions>("functions")
    .WithReference(db)
    .WaitFor(migrations)
    .WithReference(serviceBus)
    .WithReference(storage);

builder.Build().Run();
