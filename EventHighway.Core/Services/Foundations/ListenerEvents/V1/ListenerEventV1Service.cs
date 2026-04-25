// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Services.Foundations.ListernEvents.V1
{
    internal partial class ListenerEventV1Service : IListenerEventV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV1> AddListenerEventAsync(ListenerEventV1 listenerEvent) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventOnAddAsync(listenerEvent);

            return await storageBroker.InsertListenerEventV1Async(listenerEvent);
        });

        public ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllListenerEventsV1Async());

        public ValueTask<ListenerEventV1> ModifyListenerEventAsync(ListenerEventV1 listenerEvent) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventOnModifyAsync(listenerEvent);

            ListenerEventV1 maybeListenerEvent =
                await this.storageBroker.SelectListenerEventByIdV1Async(
                    listenerEvent.Id);

            ValidateListenerEventAgainstStorage(listenerEvent, maybeListenerEvent);

            return await storageBroker.UpdateListenerEventV1Async(listenerEvent);
        });

        public ValueTask<ListenerEventV1> RemoveListenerEventByIdAsync(Guid listenerEventId) =>
        TryCatch(async () =>
        {
            ValidateListenerEventId(listenerEventId);

            ListenerEventV1 maybeListenerEvent =
                await this.storageBroker.SelectListenerEventByIdV1Async(listenerEventId);

            ValidateListenerEventExists(maybeListenerEvent, listenerEventId);

            return await this.storageBroker.DeleteListenerEventV1Async(maybeListenerEvent);
        });
    }
}
