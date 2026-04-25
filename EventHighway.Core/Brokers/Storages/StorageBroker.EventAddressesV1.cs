// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventAddressV1> EventAddressesV1 { get; set; }

        public async ValueTask<EventAddressV1> InsertEventAddressV1Async(EventAddressV1 eventAddress) =>
            await InsertAsync(eventAddress);

        public async ValueTask<IQueryable<EventAddressV1>> SelectAllEventAddressesV1Async() =>
            SelectAll<EventAddressV1>();

        public async ValueTask<EventAddressV1> SelectEventAddressByIdV1Async(Guid eventAddressId) =>
            await SelectAsync<EventAddressV1>(eventAddressId);

        public async ValueTask<EventAddressV1> DeleteEventAddressV1Async(EventAddressV1 eventAddress) =>
            await DeleteAsync(eventAddress);
    }
}
