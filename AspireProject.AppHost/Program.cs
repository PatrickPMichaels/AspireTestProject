var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("Test-SB")
    .RunAsEmulator();
    
var queue = serviceBus.AddServiceBusQueue("Api-Function");
serviceBus.AddServiceBusQueue("Inter-Function");
serviceBus.AddServiceBusQueue("No-Consumer");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var blobs = storage.AddBlobs("Aspire-Blobs");

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
    .WithReference(queue)
    .WithReference(blobs)
    .WithExternalHttpEndpoints();

builder.Build().Run();
