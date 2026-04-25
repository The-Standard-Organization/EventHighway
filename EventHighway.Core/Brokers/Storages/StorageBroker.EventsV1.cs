// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventV1> EventsV1 { get; set; }

        public async ValueTask<EventV1> InsertEventV1Async(EventV1 @event) =>
            await InsertAsync(@event);

        public async ValueTask<IQueryable<EventV1>> SelectAllEventsV1Async() =>
            SelectAll<EventV1>();

        public async ValueTask<IQueryable<EventV1>> SelectAllEventsWithListenerEventsV1Async()
        {
            return SelectAll<EventV1>().Include(@event =>
                @event.ListenerEvents);
        }

        public async ValueTask<EventV1> SelectEventByIdV1Async(Guid eventId) =>
            await SelectAsync<EventV1>(eventId);

        public async ValueTask<EventV1> UpdateEventV1Async(EventV1 @event) =>
            await UpdateAsync(@event);

        public async ValueTask<EventV1> DeleteEventV1Async(EventV1 @event) =>
            await DeleteAsync(@event);
    }
}
