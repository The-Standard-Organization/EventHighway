// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V2
{
    internal partial class EventListenerV2OrchestrationService : IEventListenerV2OrchestrationService
    {
        private readonly IEventListenerV2ProcessingService eventListenerV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV2OrchestrationService(
            IEventListenerV2ProcessingService eventListenerV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventListenerV2ProcessingService = eventListenerV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventListenerV2> AddEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
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
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventAddressId(eventAddressId);

            return await this.eventListenerV2ProcessingService
                .RetrieveEventListenerV2sByEventAddressIdAsync(
                    eventAddressId,
                    cancellationToken);
        });

        public ValueTask<IReadOnlyList<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
            Guid eventAddressId,
            EventListenerV2Query eventListenerV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventListenerV2> eventListenerV2s =
                await this.eventListenerV2ProcessingService
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventAddressId,
                        cancellationToken);

            return ApplyEventListenerV2Query(eventListenerV2s, eventListenerV2Query);
        });

        private static IReadOnlyList<EventListenerV2> ApplyEventListenerV2Query(
            IQueryable<EventListenerV2> eventListenerV2s,
            EventListenerV2Query eventListenerV2Query)
        {
            if (eventListenerV2Query.HandlerId is not null)
            {
                eventListenerV2s = eventListenerV2s.Where(eventListenerV2 =>
                    eventListenerV2.HandlerId == eventListenerV2Query.HandlerId);
            }

            if (eventListenerV2Query.EventParticipantV2Id is not null)
            {
                eventListenerV2s = eventListenerV2s.Where(eventListenerV2 =>
                    eventListenerV2.EventParticipantV2Id == eventListenerV2Query.EventParticipantV2Id);
            }

            if (eventListenerV2Query.Name is not null)
            {
                eventListenerV2s = eventListenerV2s.Where(eventListenerV2 =>
                    eventListenerV2.Name == eventListenerV2Query.Name);
            }

            if (eventListenerV2Query.CreatedFrom is not null)
            {
                eventListenerV2s = eventListenerV2s.Where(eventListenerV2 =>
                    eventListenerV2.CreatedDate >= eventListenerV2Query.CreatedFrom);
            }

            if (eventListenerV2Query.CreatedTo is not null)
            {
                eventListenerV2s = eventListenerV2s.Where(eventListenerV2 =>
                    eventListenerV2.CreatedDate <= eventListenerV2Query.CreatedTo);
            }

            return eventListenerV2s
                .OrderByDescending(eventListenerV2 => eventListenerV2.CreatedDate)
                .ThenBy(eventListenerV2 => eventListenerV2.Id)
                .Skip(eventListenerV2Query.Skip)
                .Take(eventListenerV2Query.Take)
                .ToList();
        }

        public ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventListenerV2Id(eventListenerV2Id);

            return await this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                eventListenerV2Id,
                cancellationToken);
        });

        public ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventListenerV2IsNotNull(eventListenerV2);

            return await this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                eventListenerV2,
                cancellationToken);
        });
    }
}
