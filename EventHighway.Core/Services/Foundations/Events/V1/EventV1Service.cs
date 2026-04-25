// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
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

        public ValueTask<EventV1> AddEventAsync(EventV1 @event) =>
        TryCatch(async () =>
        {
            await ValidateEventOnAddAsync(@event);

            return await storageBroker.InsertEventV1Async(@event);
        });

        public ValueTask<IQueryable<EventV1>> RetrieveAllEventsAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventsV1Async());

        public ValueTask<IQueryable<EventV1>> RetrieveAllEventsWithListenerEventsAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventsWithListenerEventsV1Async());

        public ValueTask<EventV1> ModifyEventAsync(EventV1 @event) =>
        TryCatch(async () =>
        {
            await ValidateEventOnModifyAsync(@event);

            EventV1 maybeEvent =
                await this.storageBroker.SelectEventByIdV1Async(
                    @event.Id);

            ValidateEventAgainstStorage(@event, maybeEvent);

            return await storageBroker.UpdateEventV1Async(@event);
        });

        public ValueTask<EventV1> RemoveEventByIdAsync(Guid eventId) =>
        TryCatch(async () =>
        {
            ValidateEventId(eventId);

            EventV1 maybeEvent =
                await this.storageBroker.SelectEventByIdV1Async(eventId);

            ValidateEventExists(maybeEvent, eventId);

            return await this.storageBroker.DeleteEventV1Async(maybeEvent);
        });
    }
}
