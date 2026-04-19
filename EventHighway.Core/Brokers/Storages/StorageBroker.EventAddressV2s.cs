// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventAddressV2> EventAddressV2s { get; set; }

        public async ValueTask<EventAddressV2> InsertEventAddressV2Async(EventAddressV2 eventAddressV2) =>
            await InsertAsync(eventAddressV2);

        public async ValueTask<IQueryable<EventAddressV2>> SelectAllEventAddressV2sAsync() =>
            SelectAll<EventAddressV2>();

        public async ValueTask<EventAddressV2> SelectEventAddressV2ByIdAsync(Guid eventAddressV2Id) =>
            await SelectAsync<EventAddressV2>(eventAddressV2Id);

        public async ValueTask<EventAddressV2> DeleteEventAddressV2Async(EventAddressV2 eventAddressV2) =>
            await DeleteAsync(eventAddressV2);
    }
}
