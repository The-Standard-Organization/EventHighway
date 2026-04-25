// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListenerV2> EventListenersV2 { get; set; }

        public async ValueTask<EventListenerV2> InsertEventListenerV2Async(EventListenerV2 eventListener) =>
            await InsertAsync(eventListener);

        public async ValueTask<IQueryable<EventListenerV2>> SelectAllEventListenersV2Async() =>
            SelectAll<EventListenerV2>();

        public async ValueTask<EventListenerV2> SelectEventListenerByIdV2Async(Guid eventListenerId) =>
            await SelectAsync<EventListenerV2>(eventListenerId);

        public async ValueTask<EventListenerV2> DeleteEventListenerV2Async(EventListenerV2 eventListener) =>
            await DeleteAsync(eventListener);
    }
}
