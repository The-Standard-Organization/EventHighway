// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Services.Foundations.EventListeners.V1
{
    internal partial class EventListenerV1Service : IEventListenerV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventListenerV1> AddEventListenerAsync(EventListenerV1 eventListener) =>
        TryCatch(async () =>
        {
            await ValidateEventListenerV1OnAddAsync(eventListener);

            return await this.storageBroker.InsertEventListenerV1Async(eventListener);
        });

        public ValueTask<IQueryable<EventListenerV1>> RetrieveAllEventListenersAsync() =>
        TryCatch(async () => await storageBroker.SelectAllEventListenersV1Async());

        public ValueTask<EventListenerV1> RemoveEventListenerByIdAsync(Guid eventListenerId) =>
        TryCatch(async () =>
        {
            ValidateEventListenerId(eventListenerId);

            EventListenerV1 maybeEventListenerV1 =
                await this.storageBroker.SelectEventListenerByIdV1Async(eventListenerId);

            ValidateEventListenerExists(maybeEventListenerV1, eventListenerId);

            return await this.storageBroker.DeleteEventListenerV1Async(maybeEventListenerV1);
        });
    }
}
