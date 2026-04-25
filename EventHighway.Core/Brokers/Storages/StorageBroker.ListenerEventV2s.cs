// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventV2> ListenerEventsV2 { get; set; }

        public async ValueTask<ListenerEventV2> InsertListenerEventV2Async(ListenerEventV2 listenerEvent) =>
            await InsertAsync(listenerEvent);

        public async ValueTask<IQueryable<ListenerEventV2>> SelectAllListenerEventsV2Async() =>
            SelectAll<ListenerEventV2>();

        public async ValueTask<ListenerEventV2> SelectListenerEventByIdV2Async(Guid listenerEventId) =>
            await SelectAsync<ListenerEventV2>(listenerEventId);

        public async ValueTask<ListenerEventV2> UpdateListenerEventV2Async(ListenerEventV2 listenerEvent) =>
            await UpdateAsync(listenerEvent);

        public async ValueTask<ListenerEventV2> DeleteListenerEventV2Async(ListenerEventV2 listenerEvent) =>
            await DeleteAsync(listenerEvent);
    }
}
