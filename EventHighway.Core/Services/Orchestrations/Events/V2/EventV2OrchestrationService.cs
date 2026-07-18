// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Hashings;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Services.Processings.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.Events.V2
{
    internal partial class EventV2OrchestrationService : IEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IEventAddressV2ProcessingService eventAddressV2ProcessingService;
        private readonly IHashBroker hashBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IEventAddressV2ProcessingService eventAddressV2ProcessingService,
            IHashBroker hashBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.eventAddressV2ProcessingService = eventAddressV2ProcessingService;
            this.hashBroker = hashBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2ProcessingService.RetrieveAllEventV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2ProcessingService
                .RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken);
        });

        public ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2ProcessingService
                .RetrieveEventV2ByIdAsync(eventV2Id, cancellationToken);
        });

        public ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            EventAddressV2 maybeEventAddressV2 =
                await this.eventAddressV2ProcessingService
                    .RetrieveEventAddressV2ByIdAsync(
                        eventV2.EventAddressV2Id,
                        cancellationToken);

            ValidateEventAddressV2Exists(
                maybeEventAddressV2,
                eventV2.EventAddressV2Id);

            return await this.eventV2ProcessingService
                .AddEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<EventV2> StampContentHashAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            string cleanedContent =
                await this.eventV2ProcessingService
                    .RemoveVolatilePathsAsync(eventV2, cancellationToken);

            ValidateCleanedContent(cleanedContent);

            eventV2.ContentHash =
                this.hashBroker.GenerateSha256Hash(cleanedContent);

            return eventV2;
        });

        public ValueTask<IQueryable<EventV2>> RetrieveScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2ProcessingService
                .RetrieveScheduledPendingEventV2sAsync(cancellationToken);
        });

        public ValueTask<EventV2> MarkEventV2AsImmediateAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2ProcessingService
                .MarkEventV2AsImmediateAsync(eventV2, cancellationToken);
        });

        public ValueTask<int> TryClaimScheduledEventV2Async(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2ProcessingService
                .TryClaimScheduledEventV2Async(eventV2Id, cancellationToken);
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2ProcessingService
                .RemoveEventV2ByIdAsync(eventV2Id, cancellationToken);
        });

        public ValueTask<bool> IsLoopDetectedAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2ProcessingService
                .IsLoopDetectedAsync(eventV2, cancellationToken);
        });
    }
}
