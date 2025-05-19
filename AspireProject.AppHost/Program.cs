var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("TestSB")
    .RunAsEmulator(c => c.WithContainerName("Aspire-ServiceBus"));
    
var queue = serviceBus.AddServiceBusQueue("ApiFunction");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(c => c.WithContainerName("Aspire-Storage"));

var blobs = storage.AddBlobs("AspireBlobs");

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
    .WithReference(serviceBus)
    .WaitFor(queue);

builder.AddNpmApp("reactvite", "../frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints();

builder.AddAzureFunctionsProject<Projects.Functions>("functions")
    .WithReference(db)
    .WaitFor(migrations)
    .WithReference(serviceBus)
    .WaitFor(queue)
    .WithReference(blobs)
    .WaitFor(blobs);

builder.Build().Run();
