// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Foundations.Events.V1
{
    internal partial class EventV1Service : IEventV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV1> AddEventV1Async(EventV1 eventV1) =>
        TryCatch(async () =>
        {
            await ValidateEventV1OnAddAsync(eventV1);

            return await storageBroker.InsertEventV1Async(eventV1);
        });

        public ValueTask<IQueryable<EventV1>> RetrieveAllEventV1sAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventV1sAsync());

        public ValueTask<IQueryable<EventV1>> RetrieveAllEventV1sWithListenersAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventV1sWithListenersAsync());

        public ValueTask<EventV1> ModifyEventV1Async(EventV1 eventV1) =>
        TryCatch(async () =>
        {
            await ValidateEventV1OnModifyAsync(eventV1);

            EventV1 maybeEventV1 =
                await this.storageBroker.SelectEventV1ByIdAsync(
                    eventV1.Id);

            ValidateEventV1AgainstStorage(eventV1, maybeEventV1);

            return await storageBroker.UpdateEventV1Async(eventV1);
        });

        public ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventV1Id(eventV1Id);

            EventV1 maybeEventV1 =
                await this.storageBroker.SelectEventV1ByIdAsync(eventV1Id);

            ValidateEventV1Exists(maybeEventV1, eventV1Id);

            return await this.storageBroker.DeleteEventV1Async(maybeEventV1);
        });
    }
}
