// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
        public DbSet<EventAddressV1> EventAddressV1s { get; set; }

        public async ValueTask<EventAddressV1> InsertEventAddressV1Async(EventAddressV1 eventAddressV1) =>
            await InsertAsync(eventAddressV1);

        public async ValueTask<IQueryable<EventAddressV1>> SelectAllEventAddressV1sAsync() =>
            SelectAll<EventAddressV1>();

        public async ValueTask<EventAddressV1> SelectEventAddressV1ByIdAsync(Guid eventAddressV1Id) =>
            await SelectAsync<EventAddressV1>(eventAddressV1Id);

        public async ValueTask<EventAddressV1> DeleteEventAddressV1Async(EventAddressV1 eventAddressV1) =>
            await DeleteAsync(eventAddressV1);
    }
}
