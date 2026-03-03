using EventHighway.Core.Clients.EventHighways;
using Microsoft.AspNetCore.Mvc;

namespace EventPublisherApi.Controllers
{
    [ApiController]
    [Route("api/archive-events")]
    public class ArchiveEventsController : ControllerBase
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public ArchiveEventsController(IEventHighwayClient eventHighwayClient) =>
            this.eventHighwayClient = eventHighwayClient;

        [HttpPost]
        public async ValueTask<ActionResult> PostAsync()
        {
            await this.eventHighwayClient.EventV1sV1.ArchiveDeadEventV1sAsync();

            return Ok(new { Message = "Dead events archived successfully." });
        }
    }
}
