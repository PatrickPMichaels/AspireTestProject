using EFModels.Models;
using Microsoft.EntityFrameworkCore;

namespace EFModels.Contexts;

public class WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> Forecasts => Set<WeatherForecast>();
}