// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V2
{
    internal interface IListenerEventArchiveV2Service
    {
        ValueTask<ListenerEventArchiveV2> AddListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveListenerEventArchiveV2sByQueryAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventArchiveV2>> BulkAddListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
