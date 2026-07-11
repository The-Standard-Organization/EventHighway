// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventHandlerV2> EventHandlerV2s { get; set; }

        public async ValueTask<EventHandlerV2> InsertEventHandlerV2Async(
            EventHandlerV2 eventHandlerV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventHandlerV2, cancellationToken);

        public async ValueTask<IQueryable<EventHandlerV2>> SelectAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventHandlerV2>(cancellationToken);

        public async ValueTask<EventHandlerV2> SelectEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventHandlerV2>(
                new object[] { eventHandlerV2Id },
                cancellationToken);

        public async ValueTask<EventHandlerV2> DeleteEventHandlerV2Async(
            EventHandlerV2 eventHandlerV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventHandlerV2, cancellationToken);
    }
}
