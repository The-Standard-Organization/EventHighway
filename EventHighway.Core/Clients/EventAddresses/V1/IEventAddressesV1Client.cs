// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Clients.EventAddresses.V1
{
    public interface IEventAddressesV1Client
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventAddressV1> RegisterEventAddressV1Async(EventAddressV1 eventAddressV1);

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<IQueryable<EventAddressV1>> RetrieveAllEventAddressV1sAsync();

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventAddressV1> RemoveEventAddressV1ByIdAsync(Guid eventAddressV1Id);
    }
}
