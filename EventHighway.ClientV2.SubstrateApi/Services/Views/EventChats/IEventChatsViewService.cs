// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats;

namespace EventHighway.ClientV2.SubstrateApi.Services.Views.EventChats
{
    /// <summary>
    /// The chat component's one and only dependency: everything the page needs to show what the
    /// highway delivered and to put something new onto it.
    /// </summary>
    public interface IEventChatsViewService
    {
        ValueTask<List<ReceivedEventView>> RetrieveReceivedEventsAsync();

        /// <summary>
        /// The /submit call this app makes on the user's behalf, spelled out — verb, url, headers
        /// and a sample body — so the same request can be copied into Postman and made by hand.
        /// </summary>
        ValueTask<SubmitEndpointView> RetrieveSubmitEndpointAsync();

        ValueTask<MediaSubmissionView> SubmitMediaItemAsync(
            string content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// A ready-to-send media item for the composer box. Each one carries a fresh id, so
        /// pressing Send twice in a row submits two distinct items rather than tripping the
        /// substrate's loop detection.
        /// </summary>
        ValueTask<string> GenerateSampleMediaItemAsync();

        /// <summary>
        /// Raised when a delivery lands, so the chat can render it as it arrives.
        /// </summary>
        event Action ReceivedEventsChanged;
    }
}
