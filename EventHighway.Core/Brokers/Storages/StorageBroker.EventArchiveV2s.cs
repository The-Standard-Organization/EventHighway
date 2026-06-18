// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventArchiveV2> EventArchiveV2s { get; set; }

        public async ValueTask<EventArchiveV2> InsertEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventArchiveV2, cancellationToken);

        public async ValueTask<IQueryable<EventArchiveV2>> SelectAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventArchiveV2>(cancellationToken);

        public async ValueTask<IQueryable<EventArchiveV2>>
            SelectAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync(
                CancellationToken cancellationToken = default) =>
                (await SelectAllAsync<EventArchiveV2>(cancellationToken))
                    .Include(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s);

        public async ValueTask<EventArchiveV2> SelectEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventArchiveV2>(new object[] { eventArchiveV2Id }, cancellationToken);

        public async ValueTask<EventArchiveV2> DeleteEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventArchiveV2, cancellationToken);

        public async ValueTask BulkInsertEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
            await BulkInsertAsync(eventArchiveV2s, true, cancellationToken);

        public async ValueTask BulkDeleteEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
            await BulkDeleteAsync(eventArchiveV2s, true, cancellationToken);

    }
}
