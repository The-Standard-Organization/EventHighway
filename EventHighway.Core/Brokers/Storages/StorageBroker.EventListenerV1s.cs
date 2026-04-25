// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListenerV1> EventListenersV1 { get; set; }

        public async ValueTask<EventListenerV1> InsertEventListenerV1Async(EventListenerV1 eventListener) =>
            await InsertAsync(eventListener);

        public async ValueTask<IQueryable<EventListenerV1>> SelectAllEventListenersV1Async() =>
            SelectAll<EventListenerV1>();

        public async ValueTask<EventListenerV1> SelectEventListenerByIdV1Async(Guid eventListenerId) =>
            await SelectAsync<EventListenerV1>(eventListenerId);

        public async ValueTask<EventListenerV1> DeleteEventListenerV1Async(EventListenerV1 eventListener) =>
            await DeleteAsync(eventListener);
    }
}
