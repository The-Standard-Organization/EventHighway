// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses;

namespace EventHighway.Core.Clients.EventAddresses
{
    public interface IEventAddressesClient
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventAddress> RegisterEventAddressAsync(EventAddress eventAddress);

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<IQueryable<EventAddress>> RetrieveAllEventAddressesAsync();

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventAddress> RetrieveEventAddressByIdAsync(Guid eventAddressId);
    }
}
