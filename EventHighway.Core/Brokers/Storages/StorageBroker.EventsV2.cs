// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventV2> EventsV2 { get; set; }

        public async ValueTask<EventV2> InsertEventV2Async(EventV2 @event) =>
            await InsertAsync(@event);

        public async ValueTask<IQueryable<EventV2>> SelectAllEventsV2Async() =>
            SelectAll<EventV2>();

        public async ValueTask<IQueryable<EventV2>> SelectAllEventsWithListenerEventsV2Async()
        {
            return SelectAll<EventV2>().Include(@event =>
                @event.ListenerEvents);
        }

        public async ValueTask<EventV2> SelectEventByIdV2Async(Guid eventId) =>
            await SelectAsync<EventV2>(eventId);

        public async ValueTask<EventV2> UpdateEventV2Async(EventV2 @event) =>
            await UpdateAsync(@event);

        public async ValueTask<EventV2> DeleteEventV2Async(EventV2 @event) =>
            await DeleteAsync(@event);
    }
}
