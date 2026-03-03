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
    [Route("api")]
    public class ScheduleEventController : ControllerBase
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public ScheduleEventController(IEventHighwayClient eventHighwayClient) =>
            this.eventHighwayClient = eventHighwayClient;

        [HttpPost("schedule-event")]
        public async ValueTask<ActionResult<EventV1>> ScheduleAsync()
        {
            Guid eventAddressId = Guid.NewGuid();
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var eventAddress = new EventAddressV1
            {
                Id = eventAddressId,
                Name = "scheduled-updates",
                Description = "Address for scheduled status updates",
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(eventAddress);

            var listener = new EventListenerV1
            {
                Id = Guid.NewGuid(),
                Name = "Status Update Listener",
                Description = "Listens for scheduled status updates",
                Endpoint = "https://localhost:7201/api/students",
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
                FirstName = "Jane",
                LastName = "Smith"
            };

            var scheduledEvent = new EventV1
            {
                Id = Guid.NewGuid(),
                Content = JsonSerializer.Serialize(studentData),
                EventAddressId = eventAddressId,
                ScheduledDate = now.AddSeconds(seconds: 30),
                CreatedDate = now,
                UpdatedDate = now
            };

            EventV1 submittedEvent = await this.eventHighwayClient.EventV1s
                .SubmitEventV1Async(scheduledEvent);

            return Ok(submittedEvent);
        }

        [HttpPost("fire-scheduled-events")]
        public async ValueTask<ActionResult> FireScheduledAsync()
        {
            await this.eventHighwayClient.EventV1s
                .FireScheduledPendingEventV1sAsync();

            return Ok();
        }
    }
}
