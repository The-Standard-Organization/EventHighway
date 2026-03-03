using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using Microsoft.AspNetCore.Mvc;

namespace EventPublisherApi.Controllers
{
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public EventsController(IEventHighwayClient eventHighwayClient) =>
            this.eventHighwayClient = eventHighwayClient;

        [HttpPost("api/v1/event-addresses")]
        public async ValueTask<ActionResult<EventAddressV1>> PostEventAddressV1Async(
            [FromBody] EventAddressV1 eventAddressV1)
        {
            EventAddressV1 registeredEventAddressV1 =
                await this.eventHighwayClient.EventAddressV1s.RegisterEventAddressV1Async(
                    eventAddressV1);

            return Ok(registeredEventAddressV1);
        }

        [HttpPost("api/v1/event-listeners")]
        public async ValueTask<ActionResult<EventListenerV1>> PostEventListenerV1Async(
            [FromBody] EventListenerV1 eventListenerV1)
        {
            EventListenerV1 registeredEventListenerV1 =
                await this.eventHighwayClient.EventListenerV1s.RegisterEventListenerV1Async(
                    eventListenerV1);

            return Ok(registeredEventListenerV1);
        }
    }
}
