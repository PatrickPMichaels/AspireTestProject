using EFModels.Contexts;
using MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.AddSqlServerDbContext<WeatherForecastDbContext>("database");

var host = builder.Build();
host.Run();
