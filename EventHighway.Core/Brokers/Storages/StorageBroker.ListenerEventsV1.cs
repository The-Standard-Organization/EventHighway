// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventV1> ListenerEventV1s { get; set; }

        public async ValueTask<ListenerEventV1> InsertListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
            await InsertAsync(listenerEventV1);

        public async ValueTask<IQueryable<ListenerEventV1>> SelectAllListenerEventsV1Async() =>
            SelectAll<ListenerEventV1>();

        public async ValueTask<ListenerEventV1> SelectListenerEventByIdV1Async(Guid listenerEventV1Id) =>
            await SelectAsync<ListenerEventV1>(listenerEventV1Id);

        public async ValueTask<ListenerEventV1> UpdateListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
            await UpdateAsync(listenerEventV1);

        public async ValueTask<ListenerEventV1> DeleteListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
            await DeleteAsync(listenerEventV1);
    }
}
