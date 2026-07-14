// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents.Exceptions;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHighway.ClientV2.SubstrateApi.Controllers
{
    /// <summary>
    /// Where the highway knocks. The unfiltered SubstrateApi listener delivers every new release
    /// here — through the JoesRestApi delegate client, whichever app happened to dispatch it —
    /// and each delivery goes straight onto the chat UI.
    /// </summary>
    [ApiController]
    [Route("receive")]
    public class ReceivedEventsController : ControllerBase
    {
        private readonly IReceivedEventService receivedEventService;

        public ReceivedEventsController(IReceivedEventService receivedEventService) =>
            this.receivedEventService = receivedEventService;

        [HttpPost]
        public async ValueTask<ActionResult> PostReceivedEventAsync()
        {
            try
            {
                // Read verbatim rather than bound to a model: the chat shows whatever arrives, and
                // a delivery this app cannot parse is still a delivery it should show.
                using var streamReader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = await streamReader.ReadToEndAsync();

                ReceivedEvent receivedEvent =
                    await this.receivedEventService.AddReceivedEventAsync(content);

                return Ok(new
                {
                    Status = "Received",
                    receivedEvent.Id,
                    receivedEvent.ReceivedDate
                });
            }
            catch (ReceivedEventValidationException receivedEventValidationException)
            {
                return BadRequest(
                    receivedEventValidationException.InnerException?.Message
                        ?? receivedEventValidationException.Message);
            }
            catch (ReceivedEventServiceException receivedEventServiceException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    receivedEventServiceException.Message);
            }
        }
    }
}
