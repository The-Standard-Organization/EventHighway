// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V2
{
    internal interface IEventListenerV2OrchestrationService
    {
        ValueTask<EventListenerV2> AddEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
            Guid eventAddressId,
            EventListenerV2Query eventListenerV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);
    }
}
