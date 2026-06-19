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
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Processings.Events.V2
{
    internal partial class EventV2ProcessingService : IEventV2ProcessingService
    {
        private readonly IEventV2Service eventV2Service;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2ProcessingService(
            IEventV2Service eventV2Service,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2Service = eventV2Service;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.eventV2Service.RetrieveAllEventV2sAsync();
        });

        public ValueTask<EventV2> AddEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2Service.AddEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveScheduledPendingEventV2sAsync() =>
        TryCatch(async () =>
        {
            IQueryable<EventV2> eventV2s =
                await this.eventV2Service.RetrieveAllEventV2sAsync();

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return eventV2s.Where(eventV2 =>
                eventV2.Type == EventTypeV2.Scheduled &&
                eventV2.ScheduledDate < now);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync() =>
        TryCatch(async () =>
        {
            IQueryable<EventV2> eventV2s =
                await this.eventV2Service.RetrieveAllEventV2sWithListenerEventV2sAsync();

            return eventV2s.Where(eventV2 =>
                eventV2.Type == EventTypeV2.Immediate
                && eventV2.RemainingRetryAttempts == 0
                && eventV2.ListenerEventV2s.All(listenerEvent =>
                    listenerEvent.Status != ListenerEventStatusV2.Pending));
        });

        public ValueTask<IEnumerable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync(int take) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveAllDeadEventV2sWithListeners(take);

            IQueryable<EventV2> eventV2s =
                await this.eventV2Service.RetrieveAllEventV2sWithListenerEventV2sAsync();

            IQueryable<EventV2> deadEventV2s = eventV2s.Where(eventV2 =>
                eventV2.Type == EventTypeV2.Immediate
                && eventV2.RemainingRetryAttempts == 0
                && eventV2.ListenerEventV2s.All(listenerEvent =>
                    listenerEvent.Status != ListenerEventStatusV2.Pending));

            return take == 0
                ? deadEventV2s.AsEnumerable()
                : deadEventV2s.Take(take).AsEnumerable();
        });

        public ValueTask<IEnumerable<EventV2>> RetrieveBatchOfDeadEventV2sAsync(int take) =>
            throw new NotImplementedException();

        public ValueTask<EventV2> MarkEventV2AsImmediateAsync(
            EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            return await SetEventV2AsImmediateAsync(eventV2, cancellationToken);
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2Service.RemoveEventV2ByIdAsync(
                eventV2Id, cancellationToken);
        });

        public ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2sIsNotNull(eventV2s);

            await this.eventV2Service.BulkRemoveEventV2sAsync(eventV2s, cancellationToken);
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
