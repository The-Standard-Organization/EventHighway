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

        public ValueTask<EventAddressV1> AddEventAddressAsync(EventAddressV1 eventAddress) =>
        TryCatch(async () =>
        {
            await ValidateEventAddressOnAddAsync(eventAddress);

            return await this.storageBroker.InsertEventAddressV1Async(eventAddress);
        });

        public ValueTask<IQueryable<EventAddressV1>> RetrieveAllEventAddressesAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventAddressesV1Async());

        public ValueTask<EventAddressV1> RetrieveEventAddressByIdAsync(Guid eventAddressV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressV1Id);

            return await this.storageBroker.SelectEventAddressByIdV1Async(eventAddressV1Id);
        });

        public ValueTask<EventAddressV1> RemoveEventAddressByIdAsync(Guid eventAddressV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressV1Id);

            EventAddressV1 maybeEventAddressV1 =
                await this.storageBroker.SelectEventAddressByIdV1Async(eventAddressV1Id);

            ValidateEventAddressExists(maybeEventAddressV1, eventAddressV1Id);

            return await this.storageBroker.DeleteEventAddressV1Async(maybeEventAddressV1);
        });
    }
}
