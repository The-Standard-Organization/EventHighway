// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<EventListenerV2> RegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressV2Id,
            EventListenerV2Query eventListenerV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);
    }
}
