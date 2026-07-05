// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Processings.Events.V2
{
    internal partial class EventV2ProcessingService : IEventV2ProcessingService
    {
        private readonly IEventV2Service eventV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2ProcessingService(
            IEventV2Service eventV2Service,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2Service = eventV2Service;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2Service.RetrieveAllEventV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventV2Service
                .RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken);
        });

        public ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2Service.RetrieveEventV2ByIdAsync(
                eventV2Id, cancellationToken);
        });

        public ValueTask<EventV2> AddEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2Service.AddEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventV2> eventV2s =
                await this.eventV2Service.RetrieveAllEventV2sAsync(cancellationToken);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return eventV2s.Where(eventV2 =>
                eventV2.Type == EventTypeV2.Scheduled &&
                eventV2.ScheduledDate < now);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventV2> eventV2s =
                await this.eventV2Service.RetrieveAllEventV2sAsync(cancellationToken);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            RetryConfiguration retryConfiguration =
                this.configurationBroker.GetRetryConfiguration();

            DateTimeOffset cutoff =
                now.AddMinutes(-retryConfiguration.DeadAfterMinutes);

            return eventV2s.Where(eventV2 =>
                eventV2.Type == EventTypeV2.Immediate
                && eventV2.ListenerEventV2s.All(listenerEvent =>
                    listenerEvent.Status != ListenerEventStatusV2.Pending
                    && listenerEvent.Status != ListenerEventStatusV2.Replay)
                && eventV2.ListenerEventV2s.All(listenerEvent =>
                    listenerEvent.Status == ListenerEventStatusV2.Success
                    || (listenerEvent.RemainingRetryAttempts == 0
                        && (listenerEvent.DispatchedDate ?? listenerEvent.UpdatedDate) <= cutoff)));
        });

        public ValueTask<EventV2> MarkEventV2AsImmediateAsync(
            EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            return await SetEventV2AsImmediateAsync(eventV2, cancellationToken);
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2Service.RemoveEventV2ByIdAsync(
                eventV2Id, cancellationToken);
        });

        public ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2sIsNotNull(eventV2s);

            await this.eventV2Service.BulkRemoveEventV2sAsync(eventV2s, cancellationToken);
        });

        public ValueTask<IEnumerable<EventV2>> BulkRestoreEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2sIsNotNull(eventV2s);

            return await this.eventV2Service.BulkRestoreEventV2sAsync(eventV2s, cancellationToken);
        });

        public ValueTask<string> RemoveVolatilePathsAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2Service.RemoveVolatilePathsAsync(eventV2, cancellationToken);
        });

        public ValueTask<int> RetrieveEventV2CountBySignatureAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                eventV2, cancellationToken);
        });

        public ValueTask<bool> IsLoopDetectedAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            LoopDetection config =
                this.configurationBroker.GetLoopDetectionConfiguration();

            if (config.Enabled is false)
                return false;

            int count =
                await this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                    eventV2, cancellationToken);

            return count > config.Threshold;
        });

        private async ValueTask<EventV2> SetEventV2AsImmediateAsync(
            EventV2 eventV2, CancellationToken cancellationToken)
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            eventV2.Type = EventTypeV2.Immediate;
            eventV2.UpdatedDate = now;

            return await this.eventV2Service.ModifyEventV2Async(eventV2, cancellationToken);
        }
    }
}
