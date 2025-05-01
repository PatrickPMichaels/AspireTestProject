using System.Text;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class HandleMessage
    {
        private readonly ILogger<HandleMessage> _logger;
        private readonly BlobContainerClient _client;

        public HandleMessage(ILogger<HandleMessage> logger, BlobServiceClient client)
        {
            _logger = logger;
            _client = client.GetBlobContainerClient("testblobs");
        }

        [Function(nameof(HandleMessage))]
        public async Task<IActionResult> Run([ServiceBusTrigger("ApiFunction", Connection ="TestSB")] ServiceBusReceivedMessage sbMessage)
        {
            _logger.LogInformation("Service Bus Triggered");
            var message = Encoding.UTF8.GetString(sbMessage.Body);
            var blobGuid = Guid.NewGuid();
            await _client.CreateIfNotExistsAsync();
            await _client.UploadBlobAsync($"{blobGuid}-{message}", new MemoryStream(Encoding.UTF8.GetBytes(message)));
            return new OkObjectResult($"{message} recieved");
        }
    }
}
