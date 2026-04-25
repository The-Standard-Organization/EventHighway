// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Processings.Events.V1
{
    internal partial class EventV1ProcessingService : IEventV1ProcessingService
    {
        private readonly IEventV1Service eventV1Service;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1ProcessingService(
            IEventV1Service eventV1Service,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV1Service = eventV1Service;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV1> AddEventAsync(EventV1 @event) =>
        TryCatch(async () =>
        {
            ValidateEventIsNotNull(@event);

            return await this.eventV1Service.AddEventAsync(@event);
        });

        public ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventsAsync() =>
        TryCatch(async () =>
        {
            IQueryable<EventV1> events =
                await this.eventV1Service.RetrieveAllEventsAsync();

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return events.Where(eventV1 =>
                eventV1.Type == EventV1Type.Scheduled &&
                eventV1.ScheduledDate < now);
        });

        public ValueTask<IQueryable<EventV1>> RetrieveAllDeadEventsWithListenersAsync() =>
        TryCatch(async () =>
        {
            IQueryable<EventV1> eventV1s =
                await this.eventV1Service.RetrieveAllEventsWithListenerEventsAsync();

            return eventV1s.Where(eventV1 => eventV1.Type == EventV1Type.Immediate);
        });

        public ValueTask<EventV1> MarkEventAsImmediateAsync(EventV1 @event) =>
        TryCatch(async () =>
        {
            ValidateEventIsNotNull(@event);

            return await SetEventAsImmediateAsync(@event);
        });

        public ValueTask<EventV1> RemoveEventByIdAsync(Guid eventId) =>
        TryCatch(async () =>
        {
            ValidateEventId(eventId);

            return await this.eventV1Service.RemoveEventByIdAsync(
                eventId);
        });

        private async ValueTask<EventV1> SetEventAsImmediateAsync(EventV1 @event)
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            @event.Type = EventV1Type.Immediate;
            @event.UpdatedDate = now;

            return await this.eventV1Service.ModifyEventAsync(@event);
        }
    }
}
