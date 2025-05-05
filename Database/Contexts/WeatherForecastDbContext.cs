using EFModels.Models;
using Microsoft.EntityFrameworkCore;

namespace EFModels.Contexts;

public class WeatherForecastDbContext: DbContext
{
    public WeatherForecastDbContext(): base(new DbContextOptions<WeatherForecastDbContext>())
    {
    }

    public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options)
    {
    }

    public DbSet<WeatherForecast> Forecasts => Set<WeatherForecast>();
}