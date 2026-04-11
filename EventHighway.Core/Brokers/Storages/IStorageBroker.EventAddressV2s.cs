// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventAddressV2> InsertEventAddressV2Async(EventAddressV2 eventAddressV2);
        ValueTask<IQueryable<EventAddressV2>> SelectAllEventAddressV2sAsync();
        ValueTask<EventAddressV2> SelectEventAddressV2ByIdAsync(Guid eventAddressV2Id);
        ValueTask<EventAddressV2> DeleteEventAddressV2Async(EventAddressV2 eventAddressV2);
    }
}
