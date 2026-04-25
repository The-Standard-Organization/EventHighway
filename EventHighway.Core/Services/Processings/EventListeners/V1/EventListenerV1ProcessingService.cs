// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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

        public ValueTask<EventListenerV1> AddEventListenerV1Async(EventListenerV1 eventListenerV1) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV1IsNotNull(eventListenerV1);

            return await this.eventListenerV1Service.AddEventListenerAsync(eventListenerV1);
        });

        public ValueTask<IQueryable<EventListenerV1>> RetrieveEventListenerV1sByEventAddressIdAsync(
            Guid eventAddressId) => TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressId);

            IQueryable<EventListenerV1> eventListenerV1s =
                await this.eventListenerV1Service.RetrieveAllEventListenersAsync();

            return eventListenerV1s.Where(eventListenerV1 =>
                eventListenerV1.EventAddressId == eventAddressId);
        });

        public ValueTask<EventListenerV1> RemoveEventListenerV1ByIdAsync(Guid eventListenerV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV1Id(eventListenerV1Id);

            return await this.eventListenerV1Service.RemoveEventListenerByIdAsync(
                eventListenerV1Id);
        });
    }
}
