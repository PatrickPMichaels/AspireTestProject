using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureBlobClient("AspireBlobs");

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
