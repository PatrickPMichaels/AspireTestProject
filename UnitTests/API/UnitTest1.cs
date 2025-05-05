namespace UnitTests.API;

using Azure.Messaging.ServiceBus;
using EFModels.Contexts;
using EFModels.Models;
using global::API.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

[TestFixture]
[TestOf(typeof(WeatherForecastController))]
public class WeatherForecastControllerTests
{
    private Mock<ILogger<WeatherForecastController>> mockLogger;
    private Mock<WeatherForecastDbContext> mockWeatherForecastDbContext;
    private Mock<ServiceBusClient> mockServiceBusClient;

    [SetUp]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<WeatherForecastController>>();
        mockWeatherForecastDbContext = new Mock<WeatherForecastDbContext>();
        mockServiceBusClient = new Mock<ServiceBusClient>();
    }

    [Test]
    public void SaveForecast_AddsTocontext()
    {
        // Arrange
        mockWeatherForecastDbContext.Setup(c => c.AddRange(It.IsAny<IEnumerable<WeatherForecast>>())).Verifiable();
        mockWeatherForecastDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Verifiable();
        var sut = CreateSUT();

        // Act
        var response = sut.SaveForecast([new WeatherForecast { }]);

        // Assert
        Assert.That(response, Is.Not.Null);
        mockWeatherForecastDbContext.Verify();
    }
    
    [Test]
    public void SendMessageOnBus_CallsServiceBusClient()
    {
        // Arrange
        var mockSender = new Mock<ServiceBusSender>();
        mockSender.Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>())).Verifiable();
        mockServiceBusClient.Setup(c => c.CreateSender(It.IsAny<string>())).Returns(mockSender.Object).Verifiable();
        var sut = CreateSUT();

        // Act
        var response = sut.SendMessageOnBus("Test");

        // Assert
        Assert.That(response, Is.Not.Null);
        mockServiceBusClient.Verify();
        mockSender.Verify();
    }


    private WeatherForecastController CreateSUT()
    {
        return new WeatherForecastController(mockLogger.Object, mockWeatherForecastDbContext.Object, mockServiceBusClient.Object);
    }
}