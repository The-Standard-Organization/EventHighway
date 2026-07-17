// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventAddresses
{
    public partial class EventAddressesViewService : IEventAddressesViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressesViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventAddressView>> RetrieveAllAddressesAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            var eventAddressV2Query = new EventAddressV2Query { Take = 1000 };
            var addresses = new List<EventAddressV2>();

            while (true)
            {
                IReadOnlyList<EventAddressV2> addressPage =
                    await this.eventHighwayBroker.RetrieveAllEventAddressV2sAsync(
                        eventAddressV2Query, cancellationToken);

                addresses.AddRange(addressPage);

                if (addressPage.Count < eventAddressV2Query.Take)
                {
                    break;
                }

                eventAddressV2Query.Skip += eventAddressV2Query.Take;
            }

            return addresses.Select(AsView).ToList();
        });

        public ValueTask<EventAddressView> RegisterAddressAsync(
            EventAddressView address,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            DateTimeOffset now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var addressToRegister = new EventAddressV2
            {
                Id = Guid.NewGuid(),
                Name = address.Name,
                Description = address.Description,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventAddressV2 registeredAddress =
                await this.eventHighwayBroker.RegisterEventAddressV2Async(
                    addressToRegister, cancellationToken);

            return AsView(registeredAddress);
        });

        public ValueTask<EventAddressView> RemoveAddressByIdAsync(
            Guid addressId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventAddressV2 removedAddress =
                await this.eventHighwayBroker.RemoveEventAddressV2ByIdAsync(
                    addressId, cancellationToken);

            return AsView(removedAddress);
        });

        private static EventAddressView AsView(EventAddressV2 address) =>
            new EventAddressView
            {
                Id = address.Id,
                Name = address.Name,
                Description = address.Description
            };
    }
}
