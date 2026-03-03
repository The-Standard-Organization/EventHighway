using System.Text.Json;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventPublisherApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventPublisherApi.Controllers
{
    [ApiController]
    [Route("api/retry-event")]
    public class RetryEventController : ControllerBase
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public RetryEventController(IEventHighwayClient eventHighwayClient) =>
            this.eventHighwayClient = eventHighwayClient;

        [HttpPost]
        public async ValueTask<ActionResult<EventV1>> PostAsync()
        {
            Guid eventAddressId = Guid.NewGuid();
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var eventAddress = new EventAddressV1
            {
                Id = eventAddressId,
                Name = "retry-events",
                Description = "Address for retry demo events",
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(eventAddress);

            var listener = new EventListenerV1
            {
                Id = Guid.NewGuid(),
                Name = "Retry Test Listener",
                Description = "Listens for retry test events",
                Endpoint = "https://localhost:7201/api/retry-test",
                HeaderSecret = "demo-secret-123",
                EventAddressId = eventAddressId,
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(listener);

            var studentData = new Student
            {
                Id = Guid.NewGuid(),
                FirstName = "Mike",
                LastName = "Tyson"
            };

            var retryEvent = new EventV1
            {
                Id = Guid.NewGuid(),
                Content = JsonSerializer.Serialize(studentData),
                EventAddressId = eventAddressId,
                RetryAttempts = 5,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventV1 submittedEvent = await this.eventHighwayClient.EventV1s
                .SubmitEventV1AsyncV1(retryEvent);

            return Ok(submittedEvent);
        }
    }
}
