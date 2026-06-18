// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListenerArchiveV2> EventListenerArchiveV2s { get; set; }

        public async ValueTask<EventListenerArchiveV2> InsertEventListenerArchiveV2Async(
            EventListenerArchiveV2 eventListenerArchiveV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventListenerArchiveV2, cancellationToken);

        public async ValueTask<IQueryable<EventListenerArchiveV2>> SelectAllEventListenerArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventListenerArchiveV2>(cancellationToken);

        public async ValueTask InsertBulkEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default) =>
            await BulkInsertAsync(eventListenerArchiveV2s, true, cancellationToken);

        public async ValueTask DeleteBulkEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default) =>
            await BulkDeleteAsync(eventListenerArchiveV2s, true, cancellationToken);
    }
}
