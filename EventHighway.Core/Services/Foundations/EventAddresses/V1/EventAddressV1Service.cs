// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Services.Foundations.EventAddresses.V1
{
    internal partial class EventAddressV1Service : IEventAddressV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventAddressV1> AddEventAddressV1Async(EventAddressV1 eventAddressV1) =>
        TryCatch(async () =>
        {
            await ValidateEventAddressV1OnAddAsync(eventAddressV1);

            return await this.storageBroker.InsertEventAddressV1Async(eventAddressV1);
        });

        public ValueTask<IQueryable<EventAddressV1>> RetrieveAllEventAddressV1sAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventAddressesV1Async());

        public ValueTask<EventAddressV1> RetrieveEventAddressV1ByIdAsync(Guid eventAddressV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventAddressV1Id(eventAddressV1Id);

            return await this.storageBroker.SelectEventAddressByIdV1Async(eventAddressV1Id);
        });

        public ValueTask<EventAddressV1> RemoveEventAddressV1ByIdAsync(Guid eventAddressV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventAddressV1Id(eventAddressV1Id);

            EventAddressV1 maybeEventAddressV1 =
                await this.storageBroker.SelectEventAddressByIdV1Async(eventAddressV1Id);

            ValidateEventAddressV1Exists(maybeEventAddressV1, eventAddressV1Id);

            return await this.storageBroker.DeleteEventAddressV1Async(maybeEventAddressV1);
        });
    }
}
