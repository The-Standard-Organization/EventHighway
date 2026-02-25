// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Services.Foundations.EventAddresses.V1
{
    internal interface IEventAddressV1Service
    {
        ValueTask<EventAddressV1> AddEventAddressV1Async(EventAddressV1 eventAddressV1);
        ValueTask<IQueryable<EventAddressV1>> RetrieveAllEventAddressV1sAsync();
        ValueTask<EventAddressV1> RetrieveEventAddressV1ByIdAsync(Guid eventAddressV1Id);
        ValueTask<EventAddressV1> RemoveEventAddressV1ByIdAsync(Guid eventAddressV1Id);
    }
}
