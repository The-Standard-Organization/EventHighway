// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ExternalMediaItems.Exceptions;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.ExternalMediaItems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHighway.ClientV2.SubstrateApi.Controllers
{
    /// <summary>
    /// The public intake — the same contribution path the SubstrateApp console sample calls
    /// in-process, opened up over HTTP so Postman (or this app's own chat box) can reach it.
    /// The contributor's credentials are read from the request headers and never from the body:
    /// a body is data a caller can pass around, headers are how a caller identifies itself.
    /// </summary>
    [ApiController]
    [Route("submit")]
    public class ExternalMediaItemsController : ControllerBase
    {
        private const string ParticipantHeader = "X-EventHighwayParticipant";
        private const string ParticipantSecretHeader = "X-EventHighwayParticipantSecret";

        private readonly IExternalMediaItemService externalMediaItemService;

        public ExternalMediaItemsController(IExternalMediaItemService externalMediaItemService) =>
            this.externalMediaItemService = externalMediaItemService;

        [HttpPost]
        public async ValueTask<ActionResult> PostExternalMediaItemAsync(
            [FromBody] MediaItem mediaItem,
            [FromHeader(Name = ParticipantHeader)] string participantId,
            [FromHeader(Name = ParticipantSecretHeader)] string participantSecret)
        {
            try
            {
                await this.externalMediaItemService.AddExternalMediaItemAsync(
                    mediaItem,
                    participantId,
                    participantSecret);

                // Accepted, not Created: what this endpoint owns is a submission onto the highway.
                // The media item itself is created downstream, by whoever is listening for it.
                return Accepted(new
                {
                    Status = "Accepted",
                    mediaItem.Id,
                    mediaItem.Title
                });
            }
            catch (ExternalMediaItemValidationException externalMediaItemValidationException)
            {
                return BadRequest(DescribeRootCause(externalMediaItemValidationException));
            }
            catch (ExternalMediaItemDependencyValidationException
                externalMediaItemDependencyValidationException)
            {
                return BadRequest(DescribeRootCause(externalMediaItemDependencyValidationException));
            }
            catch (ExternalMediaItemDependencyException externalMediaItemDependencyException)
            {
                return InternalServerError(externalMediaItemDependencyException);
            }
            catch (ExternalMediaItemServiceException externalMediaItemServiceException)
            {
                return InternalServerError(externalMediaItemServiceException);
            }
        }

        // The root cause is the part a contributor can act on ("Event participant not found",
        // or "Title: Text is required"); the layers wrapping it only say who noticed. Xeptions
        // carry the offending fields in their data list, so those are spelled out too.
        private static string DescribeRootCause(Exception exception)
        {
            Exception rootException = exception;

            while (rootException.InnerException is not null)
                rootException = rootException.InnerException;

            List<string> invalidFields = rootException.Data
                .Cast<DictionaryEntry>()
                .Select(entry => $"{entry.Key}: {DescribeValue(entry.Value)}")
                .ToList();

            return invalidFields.Any()
                ? $"{rootException.Message} ({string.Join("; ", invalidFields)})"
                : rootException.Message;
        }

        private static string DescribeValue(object value) =>
            value is IEnumerable<string> messages
                ? string.Join(", ", messages)
                : value?.ToString();

        private ObjectResult InternalServerError(Exception exception) =>
            StatusCode(
                StatusCodes.Status500InternalServerError,
                exception.Message);
    }
}
