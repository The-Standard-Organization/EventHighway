// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems.Exceptions;
using EventHighway.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaItems
{
    // The substrate seam — MediaItemService's ONLY subscription, wired to the listener on
    // "NFlix-ExternalContributions". The substrate invokes the delegate with the raw event
    // content — never the contributing participant's id or secret — and the handler binds it
    // to a typed model, funnels it through AddMediaItemAsync and maps outcomes (including
    // exceptions) to an EventHandlerResult.
    internal partial class MediaItemService
    {
        private DelegateEventHandler externalMediaItemAddedEventHandler;

        public IEventHandler ExternalMediaItemAddedEventHandler =>
            this.externalMediaItemAddedEventHandler ??= new DelegateEventHandler(
                SeedIdentifiers.MediaItemServiceHandler,
                HandleExternalMediaItemAddedAsync,
                name: "MediaItemService");

        private async ValueTask<EventHandlerResult> HandleExternalMediaItemAddedAsync(
            string content,
            CancellationToken cancellationToken)
        {
            try
            {
                MediaItem mediaItem =
                    await this.jsonSerializationBroker.DeserializeAsync<MediaItem>(content);

                MediaItem addedMediaItem = await AddMediaItemAsync(mediaItem);

                this.loggingBroker.LogInformation(
                    $"MediaItemService ingested {addedMediaItem.Title} " +
                    $"({addedMediaItem.Type} - {addedMediaItem.Rating} rating) " +
                    "and relayed MediaItemAdded");

                return new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = addedMediaItem.Title,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                };
            }
            catch (MediaItemValidationException mediaItemValidationException)
            {
                return new EventHandlerResult
                {
                    IsSuccess = false,
                    Response = mediaItemValidationException.InnerException?.Message ?? string.Empty,
                    ResponseCode = "400",
                    ResponseMessage = "Bad Request"
                };
            }
            catch (Exception exception)
            {
                return new EventHandlerResult
                {
                    IsSuccess = false,
                    Response = exception.Message,
                    ResponseCode = "500",
                    ResponseMessage = "Internal Server Error"
                };
            }
        }
    }
}
