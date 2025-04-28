var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    //.WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("dbVolume");

var serviceBus = builder.AddAzureServiceBus("Queue")
    .RunAsEmulator();

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var events = builder.AddAzureEventHubs("events")
    .RunAsEmulator();

var db = sql.AddDatabase("database");

var api = builder.AddProject<Projects.API>("api")
    .WithReference(db);

builder.AddNpmApp("reactvite", "../frontend")
    .WithReference(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddProject<Projects.Functions>("functions");

builder.Build().Run();
