// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ListenerEvents.V2
{
    internal interface IListenerEventV2OrchestrationService
    {
        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveListenerEventV2sByQueryAsync(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveListenerEventV2sWithEventListenerV2ByQueryAsync(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfListenerEventV2sByEventIdsAsync(
            IEnumerable<Guid> eventV2Ids,
            int take,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> ResetRetriesForListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);
    }
}
