// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventV2> ListenerEventV2s { get; set; }

        public async ValueTask<ListenerEventV2> InsertListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(listenerEventV2, cancellationToken);

        public async ValueTask<IQueryable<ListenerEventV2>> SelectAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<ListenerEventV2>(cancellationToken);

        public async ValueTask<ListenerEventV2> SelectListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<ListenerEventV2>(new object[] { listenerEventV2Id }, cancellationToken);

        public async ValueTask<ListenerEventV2> UpdateListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(listenerEventV2, cancellationToken);

        public async ValueTask<ListenerEventV2> DeleteListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(listenerEventV2, cancellationToken);

        public async ValueTask InsertBulkListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
            await BulkInsertAsync(listenerEventV2s, true, cancellationToken);

        public async ValueTask DeleteBulkListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
            await BulkDeleteAsync(listenerEventV2s, true, cancellationToken);
    }
}
