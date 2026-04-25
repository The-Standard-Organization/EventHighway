// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventAddressV1> InsertEventAddressV1Async(EventAddressV1 eventAddress);
        ValueTask<IQueryable<EventAddressV1>> SelectAllEventAddressesV1Async();
        ValueTask<EventAddressV1> SelectEventAddressByIdV1Async(Guid eventAddressId);
        ValueTask<EventAddressV1> DeleteEventAddressV1Async(EventAddressV1 eventAddress);
    }
}
