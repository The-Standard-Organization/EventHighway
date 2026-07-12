// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses;
using EventHighway.Core.Services.Foundations.EventAddresses;

namespace EventHighway.Core.Clients.EventAddresses
{
    public class EventAddressesClient : IEventAddressesClient
    {
        private readonly IEventAddressService eventAddressService;

        public EventAddressesClient(IEventAddressService eventAddressService) =>
            this.eventAddressService = eventAddressService;

        [Obsolete("This function is deprecated use the latest version instead.")]
        public async ValueTask<EventAddress> RegisterEventAddressAsync(EventAddress eventAddress) =>
            await this.eventAddressService.AddEventAddressAsync(eventAddress);

        [Obsolete("This function is deprecated use the latest version instead.")]
        public async ValueTask<IQueryable<EventAddress>> RetrieveAllEventAddressesAsync() =>
            await this.eventAddressService.RetrieveAllEventAddressesAsync();

        [Obsolete("This function is deprecated use the latest version instead.")]
        public async ValueTask<EventAddress> RetrieveEventAddressByIdAsync(Guid eventAddressId) =>
            await this.eventAddressService.RetrieveEventAddressByIdAsync(eventAddressId);
    }
}
