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
    internal partial interface IStorageBroker
    {
        DbSet<EventHandlerV2> EventHandlerV2s { get; set; }

        ValueTask<EventHandlerV2> InsertEventHandlerV2Async(
            EventHandlerV2 eventHandlerV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventHandlerV2>> SelectAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventHandlerV2> SelectEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventHandlerV2> DeleteEventHandlerV2Async(
            EventHandlerV2 eventHandlerV2,
            CancellationToken cancellationToken = default);
    }
}
