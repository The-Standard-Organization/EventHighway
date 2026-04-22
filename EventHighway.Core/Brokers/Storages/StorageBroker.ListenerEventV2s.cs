// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
        public DbSet<ListenerEventV2> ListenerEventV2s { get; set; }

        public async ValueTask<ListenerEventV2> InsertListenerEventV2Async(ListenerEventV2 listenerEventV2) =>
            await InsertAsync(listenerEventV2);

        public async ValueTask<IQueryable<ListenerEventV2>> SelectAllListenerEventV2sAsync() =>
            SelectAll<ListenerEventV2>();

        public async ValueTask<ListenerEventV2> SelectListenerEventV2ByIdAsync(Guid listenerEventV2Id) =>
            await SelectAsync<ListenerEventV2>(listenerEventV2Id);

        public async ValueTask<ListenerEventV2> UpdateListenerEventV2Async(ListenerEventV2 listenerEventV2) =>
            await UpdateAsync(listenerEventV2);

        public async ValueTask<ListenerEventV2> DeleteListenerEventV2Async(ListenerEventV2 listenerEventV2) =>
            await DeleteAsync(listenerEventV2);
    }
}
