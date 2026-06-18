// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventV2> EventV2s { get; set; }

        public async ValueTask<EventV2> InsertEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventV2, cancellationToken);

        public async ValueTask<IQueryable<EventV2>> SelectAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventV2>(cancellationToken);

        public async ValueTask<IQueryable<EventV2>> SelectAllEventV2sWithListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            (await SelectAllAsync<EventV2>(cancellationToken))
                .Include(eventV2 => eventV2.ListenerEventV2s)
                    .ThenInclude(listenerEventV2 => listenerEventV2.EventListener);

        public async ValueTask<EventV2> SelectEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventV2>(new object[] { eventV2Id }, cancellationToken);

        public async ValueTask<EventV2> UpdateEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(eventV2, cancellationToken);

        public async ValueTask<EventV2> DeleteEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventV2, cancellationToken);

        public async ValueTask BulkInsertEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
            await BulkInsertAsync(eventV2s, true, cancellationToken);

        public async ValueTask BulkDeleteEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
            await BulkDeleteAsync(eventV2s, true, cancellationToken);
    }
}
