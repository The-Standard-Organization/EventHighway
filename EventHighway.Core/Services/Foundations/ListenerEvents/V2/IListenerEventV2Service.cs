// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Foundations.ListenerEvents.V2
{
    internal interface IListenerEventV2Service
    {
        ValueTask<ListenerEventV2> AddListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> ModifyListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventV2>> BulkRestoreListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventV2>> BulkModifyListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV2>> RetrieveListenerEventV2sByEventIdsAsync(
            IEnumerable<Guid> eventIds,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventV2>>
            RetrieveReplayBatchListenerEventV2sWithEventWithEventListenerAsync(
                int take,
                CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventV2>>
            RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                int take,
                CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> RetrieveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV2>> RetrieveListenerEventV2sByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);
    }
}
