// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.ExternalMediaItems
{
    public interface IExternalMediaItemService
    {
        /// <summary>
        /// Publishes an externally contributed media item onto the substrate.
        /// <paramref name="participantId"/> and <paramref name="participantSecret"/> are the
        /// contributing participant's credentials, extracted from the HTTP client request
        /// headers — they are never part of the request body.
        /// </summary>
        ValueTask AddExternalMediaItemAsync(
            MediaItem mediaItem,
            string participantId,
            string participantSecret);
    }
}
