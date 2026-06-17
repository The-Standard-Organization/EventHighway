// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V2
{
    internal partial class EventListenerV2OrchestrationService : IEventListenerV2OrchestrationService
    {
        private readonly IEventListenerV2ProcessingService eventListenerV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IEventHandlerV2Service eventHandlerV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV2OrchestrationService(
            IEventListenerV2ProcessingService eventListenerV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IEventHandlerV2Service eventHandlerV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventListenerV2ProcessingService = eventListenerV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.eventHandlerV2Service = eventHandlerV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventListenerV2>> RetrieveAllEventListenerV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventListenerV2ProcessingService.RetrieveAllEventListenerV2sAsync());

        public ValueTask<IEnumerable<IEventHandler>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(() =>
            ValueTask.FromResult(this.eventHandlerV2Service.RetrieveAllEventHandlerV2s()));

        public ValueTask<EventListenerV2> AddEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV2IsNotNull(eventListenerV2);

            return await this.eventListenerV2ProcessingService.AddEventListenerV2Async(
                eventListenerV2,
                cancellationToken);
        });

        public ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressId);

            return await this.eventListenerV2ProcessingService
                .RetrieveEventListenerV2sByEventAddressIdAsync(
                    eventAddressId,
                    cancellationToken);
        });

        public ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV2Id(eventListenerV2Id);

            return await this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                eventListenerV2Id,
                cancellationToken);
        });

        public ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                eventListenerV2,
                cancellationToken));

        public ValueTask<ListenerEventV2> AddListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV2IsNotNull(listenerEventV2);

            return await this.listenerEventV2ProcessingService.AddListenerEventV2Async(
                listenerEventV2,
                cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            return await this.listenerEventV2ProcessingService
                .RetrieveAllListenerEventV2sAsync();
        });

        public ValueTask<ListenerEventV2> ModifyListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV2IsNotNull(listenerEventV2);

            return await this.listenerEventV2ProcessingService.ModifyListenerEventV2Async(
                listenerEventV2,
                cancellationToken);
        });

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV2Id(listenerEventV2Id);

            return await this.listenerEventV2ProcessingService
                .RemoveListenerEventV2ByIdAsync(
                    listenerEventV2Id,
                    cancellationToken);
        });
    }
}
