// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
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
        public DbSet<EventAddressV2> EventAddressesV2 { get; set; }

        public async ValueTask<EventAddressV2> InsertEventAddressV2Async(EventAddressV2 eventAddress) =>
            await InsertAsync(eventAddress);

        public async ValueTask<IQueryable<EventAddressV2>> SelectAllEventAddressesV2Async() =>
            SelectAll<EventAddressV2>();

        public async ValueTask<EventAddressV2> SelectEventAddressByIdV2Async(Guid eventAddressId) =>
            await SelectAsync<EventAddressV2>(eventAddressId);

        public async ValueTask<EventAddressV2> DeleteEventAddressV2Async(EventAddressV2 eventAddress) =>
            await DeleteAsync(eventAddress);
    }
}
