// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Services.Foundations.EventAddresses.V1
{
    internal interface IEventAddressV1Service
    {
        ValueTask<EventAddressV1> AddEventAddressAsync(EventAddressV1 eventAddress);
        ValueTask<IQueryable<EventAddressV1>> RetrieveAllEventAddressesAsync();
        ValueTask<EventAddressV1> RetrieveEventAddressByIdAsync(Guid eventAddressId);
        ValueTask<EventAddressV1> RemoveEventAddressByIdAsync(Guid eventAddressId);
    }
}
