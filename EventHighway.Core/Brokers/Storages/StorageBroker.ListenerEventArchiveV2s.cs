// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventArchiveV2> ListenerEventArchiveV2s { get; set; }

        public async ValueTask<ListenerEventArchiveV2> InsertListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(listenerEventArchiveV2, cancellationToken);

        public async ValueTask<IQueryable<ListenerEventArchiveV2>> SelectAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<ListenerEventArchiveV2>(cancellationToken);

        public async ValueTask BulkInsertListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
            await BulkInsertAsync(listenerEventArchiveV2s, true, cancellationToken);

        public async ValueTask BulkDeleteListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
            await BulkDeleteAsync(listenerEventArchiveV2s, true, cancellationToken);
    }
}
