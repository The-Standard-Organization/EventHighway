// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<ListenerEventArchiveV2> InsertListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventArchiveV2>> SelectAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask BulkInsertListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask BulkDeleteListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
