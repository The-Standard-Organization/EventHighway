// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Services.Processings.EventListeners.V1
{
    internal partial class EventListenerV1ProcessingService : IEventListenerV1ProcessingService
    {
        private readonly IEventListenerV1Service eventListenerV1Service;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV1ProcessingService(
            IEventListenerV1Service eventListenerV1Service,
            ILoggingBroker loggingBroker)
        {
            this.eventListenerV1Service = eventListenerV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventListenerV1> AddEventListenerAsync(EventListenerV1 eventListener) =>
        TryCatch(async () =>
        {
            ValidateEventListenerIsNotNull(eventListener);

            return await this.eventListenerV1Service.AddEventListenerAsync(eventListener);
        });

        public ValueTask<IQueryable<EventListenerV1>> RetrieveEventListenersByEventAddressIdAsync(
            Guid eventAddressId) => TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressId);

            IQueryable<EventListenerV1> eventListeners =
                await this.eventListenerV1Service.RetrieveAllEventListenersAsync();

            return eventListeners.Where(eventListenerV1 =>
                eventListenerV1.EventAddressId == eventAddressId);
        });

        public ValueTask<EventListenerV1> RemoveEventListenerByIdAsync(Guid eventListenerId) =>
        TryCatch(async () =>
        {
            ValidateEventListenerId(eventListenerId);

            return await this.eventListenerV1Service.RemoveEventListenerByIdAsync(
                eventListenerId);
        });
    }
}
