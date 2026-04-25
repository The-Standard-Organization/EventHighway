// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Processings.EventListeners.V1;
using EventHighway.Core.Services.Processings.ListenerEvents.V1;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V1
{
    internal partial class EventListenerV1OrchestrationService : IEventListenerV1OrchestrationService
    {
        private readonly IEventListenerV1ProcessingService eventListenerV1ProcessingService;
        private readonly IListenerEventV1ProcessingService listenerEventV1ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV1OrchestrationService(
            IEventListenerV1ProcessingService eventListenerV1ProcessingService,
            IListenerEventV1ProcessingService listenerEventV1ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventListenerV1ProcessingService = eventListenerV1ProcessingService;
            this.listenerEventV1ProcessingService = listenerEventV1ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventListenerV1> AddEventListenerAsync(EventListenerV1 eventListenerV1) =>
        TryCatch(async () =>
        {
            ValidateEventListenerIsNotNull(eventListenerV1);

            return await this.eventListenerV1ProcessingService.AddEventListenerAsync(
                eventListenerV1);
        });

        public ValueTask<IQueryable<EventListenerV1>> RetrieveEventListenersByEventAddressIdAsync(
            Guid eventAddressId) => TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressId);

            return await this.eventListenerV1ProcessingService
                .RetrieveEventListenersByEventAddressIdAsync(eventAddressId);
        });

        public ValueTask<EventListenerV1> RemoveEventListenerByIdAsync(Guid eventListenerV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventListenerId(eventListenerV1Id);

            return await this.eventListenerV1ProcessingService.RemoveEventListenerByIdAsync(
                eventListenerV1Id);
        });

        public ValueTask<ListenerEventV1> AddListenerEventAsync(ListenerEventV1 listenerEventV1) =>
        TryCatch(async () =>
        {
            ValidateListenerEventIsNotNull(listenerEventV1);

            return await this.listenerEventV1ProcessingService.AddListenerEventAsync(
                listenerEventV1);
        });

        public ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync() =>
        TryCatch(async () =>
        {
            return await this.listenerEventV1ProcessingService
                .RetrieveAllListenerEventsAsync();
        });

        public ValueTask<ListenerEventV1> ModifyListenerEventAsync(ListenerEventV1 listenerEventV1) =>
        TryCatch(async () =>
        {
            ValidateListenerEventIsNotNull(listenerEventV1);

            return await this.listenerEventV1ProcessingService.ModifyListenerEventAsync(
                listenerEventV1);
        });

        public ValueTask<ListenerEventV1> RemoveListenerEventByIdAsync(Guid listenerEventV1Id) =>
        TryCatch(async () =>
        {
            ValidateListenerEventId(listenerEventV1Id);

            return await this.listenerEventV1ProcessingService
                .RemoveListenerEventByIdAsync(listenerEventV1Id);
        });
    }
}
