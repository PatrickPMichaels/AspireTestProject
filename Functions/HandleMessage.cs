using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class HandleMessage
    {
        private readonly ILogger<HandleMessage> _logger;

        public HandleMessage(ILogger<HandleMessage> logger)
        {
            _logger = logger;
        }

        [Function(nameof(HandleMessage))]
        public IActionResult Run([ServiceBusTrigger("Api-Function", Connection ="Test-SB")] ServiceBusReceivedMessage sbMessage)
        {
            _logger.LogInformation("Service Bus Triggered");
            var message = Encoding.UTF8.GetString(sbMessage.Body);
            return new OkObjectResult($"{message} recieved");
        }
    }
}
