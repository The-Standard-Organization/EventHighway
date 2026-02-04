// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
        public DbSet<EventV1> EventV1s { get; set; }

        public async ValueTask<EventV1> InsertEventV1Async(EventV1 eventV1) =>
            await InsertAsync(eventV1);

        public async ValueTask<IQueryable<EventV1>> SelectAllEventV1sAsync() =>
            SelectAll<EventV1>();

        public async ValueTask<IQueryable<EventV1>> SelectAllEventV1sWithListenersAsync()
        {
            return SelectAll<EventV1>().Include(eventV1 =>
                eventV1.ListenerEvents);
        }

        public async ValueTask<EventV1> SelectEventV1ByIdAsync(Guid eventV1Id) =>
            await SelectAsync<EventV1>(eventV1Id);

        public async ValueTask<EventV1> UpdateEventV1Async(EventV1 eventV1) =>
            await UpdateAsync(eventV1);

        public async ValueTask<EventV1> DeleteEventV1Async(EventV1 eventV1) =>
            await DeleteAsync(eventV1);
    }
}
