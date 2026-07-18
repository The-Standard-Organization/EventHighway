// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventFirings.V2;
using EventHighway.Core.Services.Orchestrations.EventParticipants.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;

namespace EventHighway.Core.Services.Coordinations.Events.V2
{
    internal partial class EventV2CoordinationService : IEventV2CoordinationService
    {
        private readonly IEventV2OrchestrationService eventV2OrchestrationService;
        private readonly IEventFiringV2OrchestrationService eventFiringV2OrchestrationService;
        private readonly IEventParticipantV2OrchestrationService eventParticipantV2OrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2CoordinationService(
            IEventV2OrchestrationService eventV2OrchestrationService,
            IEventFiringV2OrchestrationService eventFiringV2OrchestrationService,
            IEventParticipantV2OrchestrationService eventParticipantV2OrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2OrchestrationService = eventV2OrchestrationService;
            this.eventFiringV2OrchestrationService = eventFiringV2OrchestrationService;
            this.eventParticipantV2OrchestrationService = eventParticipantV2OrchestrationService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            await this.eventParticipantV2OrchestrationService
                .ValidateEventParticipantsAsync(eventV2, cancellationToken);

            eventV2.EventParticipantV2Secret = null;

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            eventV2.Type = eventV2.ScheduledDate switch
            {
                null => EventTypeV2.Immediate,

                DateTimeOffset scheduledDate
                    when scheduledDate < now => EventTypeV2.Immediate,

                _ => EventTypeV2.Scheduled,
            };

            eventV2 = await this.eventV2OrchestrationService
                .StampContentHashAsync(eventV2, cancellationToken);

            bool isLoop = await this.eventV2OrchestrationService
                .IsLoopDetectedAsync(eventV2, cancellationToken);

            if (isLoop)
            {
                eventV2.Status = EventStatusV2.Quarantined;
            }

            EventV2 submittedEventV2 =
                await this.eventV2OrchestrationService
                    .SubmitEventV2Async(eventV2, cancellationToken);

            if (isLoop)
            {
                throw new LoopDetectedEventV2CoordinationException(
                    message: "Event loop detected, event quarantined.");
            }

            if (submittedEventV2.Type is EventTypeV2.Immediate)
            {
                submittedEventV2 = await this.eventFiringV2OrchestrationService
                    .FireEventV2Async(submittedEventV2, cancellationToken);
            }

            return submittedEventV2;
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2OrchestrationService
                .RetrieveAllEventV2sAsync(cancellationToken);
        });

        public ValueTask<IReadOnlyList<EventV2>> RetrieveEventV2sByQueryAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Query(eventV2Query);

            IQueryable<EventV2> eventV2s =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            return ApplyEventV2Query(eventV2s, eventV2Query).ToList();
        });

        public ValueTask<IReadOnlyList<EventV2>> RetrieveEventV2sWithEventAddressV2ByQueryAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Query(eventV2Query);

            IQueryable<EventV2> eventV2s =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken);

            return ApplyEventV2Query(eventV2s, eventV2Query).ToList();
        });

        private static IQueryable<EventV2> ApplyEventV2Query(
            IQueryable<EventV2> eventV2s,
            EventV2Query eventV2Query)
        {
            if (eventV2Query.EventAddressV2Id is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.EventAddressV2Id == eventV2Query.EventAddressV2Id);
            }

            if (eventV2Query.EventParticipantV2Id is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.EventParticipantV2Id == eventV2Query.EventParticipantV2Id);
            }

            if (eventV2Query.EventName is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.EventName == eventV2Query.EventName);
            }

            if (eventV2Query.Status is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.Status == eventV2Query.Status);
            }

            if (eventV2Query.Type is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.Type == eventV2Query.Type);
            }

            if (eventV2Query.CreatedFrom is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.CreatedDate >= eventV2Query.CreatedFrom);
            }

            if (eventV2Query.CreatedTo is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.CreatedDate <= eventV2Query.CreatedTo);
            }

            if (eventV2Query.ScheduledFrom is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.ScheduledDate != null
                        && eventV2.ScheduledDate >= eventV2Query.ScheduledFrom);
            }

            if (eventV2Query.ScheduledTo is not null)
            {
                eventV2s = eventV2s.Where(eventV2 =>
                    eventV2.ScheduledDate != null
                        && eventV2.ScheduledDate <= eventV2Query.ScheduledTo);
            }

            return eventV2s
                .OrderByDescending(eventV2 => eventV2.CreatedDate)
                .ThenBy(eventV2 => eventV2.Id)
                .Skip(eventV2Query.Skip)
                .Take(eventV2Query.Take);
        }

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2OrchestrationService
                .RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken);
        });

        public ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2OrchestrationService
                .RetrieveEventV2ByIdAsync(eventV2Id, cancellationToken);
        });

        public ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<EventV2> eventV2s =
                  (await this.eventV2OrchestrationService
                      .RetrieveScheduledPendingEventV2sAsync(cancellationToken))
                          .ToList();

            foreach (EventV2 eventV2 in eventV2s)
            {
                if (eventV2.Status == EventStatusV2.Quarantined)
                    continue;

                try
                {
                    await this.eventV2OrchestrationService
                        .MarkEventV2AsImmediateAsync(eventV2, cancellationToken);

                    await this.eventFiringV2OrchestrationService
                        .FireEventV2Async(eventV2, cancellationToken);
                }
                catch (EventV2OrchestrationDependencyValidationException)
                {
                    // Another sweep (or host) already claimed this scheduled event by
                    // transitioning it out of the pending state, so skip it quietly to
                    // avoid dispatching its listeners twice.
                }
                catch (Exception exception)
                    when (exception is not OperationCanceledException)
                {
                    await this.loggingBroker.LogErrorAsync(exception);
                }
            }
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2OrchestrationService
                .RemoveEventV2ByIdAsync(eventV2Id, cancellationToken);
        });
    }
}
