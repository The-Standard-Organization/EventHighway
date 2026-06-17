// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.Core.Services.Processings.EventListeners.V2
{
    internal interface IEventListenerV2ProcessingService
    {
        ValueTask<IQueryable<EventListenerV2>> RetrieveAllEventListenerV2sAsync();

        ValueTask<EventListenerV2> AddEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);
    }
}
