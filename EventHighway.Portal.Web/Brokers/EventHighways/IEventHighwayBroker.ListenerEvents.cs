// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);
    }
}
