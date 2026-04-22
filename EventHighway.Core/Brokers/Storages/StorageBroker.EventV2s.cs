// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
        public DbSet<EventV2> EventV2s { get; set; }

        public async ValueTask<EventV2> InsertEventV2Async(EventV2 eventV2) =>
            await InsertAsync(eventV2);

        public async ValueTask<IQueryable<EventV2>> SelectAllEventV2sAsync() =>
            SelectAll<EventV2>();

        public async ValueTask<IQueryable<EventV2>> SelectAllEventV2sWithListenerEventV2sAsync()
        {
            return SelectAll<EventV2>().Include(eventV2 =>
                eventV2.ListenerEvents);
        }

        public async ValueTask<EventV2> SelectEventV2ByIdAsync(Guid eventV2Id) =>
            await SelectAsync<EventV2>(eventV2Id);

        public async ValueTask<EventV2> UpdateEventV2Async(EventV2 eventV2) =>
            await UpdateAsync(eventV2);

        public async ValueTask<EventV2> DeleteEventV2Async(EventV2 eventV2) =>
            await DeleteAsync(eventV2);
    }
}
