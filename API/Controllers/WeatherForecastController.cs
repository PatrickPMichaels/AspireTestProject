using Azure.Messaging.ServiceBus;
using EFModels.Contexts;
using EFModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    ILogger<WeatherForecastController> logger, 
    WeatherForecastDbContext context,
    ServiceBusClient serviceBus
    ) 
    : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost]
    public ActionResult SaveForecast([FromBody] IEnumerable<WeatherForecast> forecast)
    {
        context.AddRange(forecast);
        context.SaveChanges();
        
        return Ok();
    }

    [HttpPost("Message")]
    public async Task<ActionResult> SendMessageOnBus([FromBody] string message) {
        await serviceBus.CreateSender("ApiFunction")
            .SendMessageAsync(new ServiceBusMessage(message));

        return Ok();
    }
}
