// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Processings.EventAddresses.V1;
using EventHighway.Core.Services.Processings.EventCalls.V1;
using EventHighway.Core.Services.Processings.Events.V1;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationService : IEventV1OrchestrationService
    {
        private readonly IEventV1ProcessingService eventV1ProcessingService;
        private readonly IEventAddressV1ProcessingService eventAddressV1ProcessingService;
        private readonly IEventCallV1ProcessingService eventCallV1ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventV1OrchestrationService(
            IEventV1ProcessingService eventV1ProcessingService,
            IEventAddressV1ProcessingService eventAddressV1ProcessingService,
            IEventCallV1ProcessingService eventCallV1ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventV1ProcessingService = eventV1ProcessingService;
            this.eventAddressV1ProcessingService = eventAddressV1ProcessingService;
            this.eventCallV1ProcessingService = eventCallV1ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV1> SubmitEventAsync(EventV1 @event) =>
        TryCatch(async () =>
        {
            ValidateEventIsNotNull(@event);

            EventAddressV1 maybeEventAddress =
                await this.eventAddressV1ProcessingService
                    .RetrieveEventAddressByIdAsync(
                        @event.EventAddressId);

            ValidateListenerEventExists(
                maybeEventAddress,
                @event.EventAddressId);

            return await this.eventV1ProcessingService
                .AddEventAsync(@event);
        });

        public ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventsAsync() =>
        TryCatch(async () =>
        {
            return await this.eventV1ProcessingService
                .RetrieveScheduledPendingEventsAsync();
        });

        public ValueTask<EventV1> MarkEventAsImmediateAsync(EventV1 @event) =>
        TryCatch(async () =>
        {
            ValidateEventIsNotNull(@event);

            return await this.eventV1ProcessingService
                .MarkEventAsImmediateAsync(@event);
        });

        public ValueTask<EventV1> RemoveEventByIdAsync(Guid eventId) =>
        TryCatch(async () =>
        {
            ValidateEventId(eventId);

            return await this.eventV1ProcessingService
                .RemoveEventByIdAsync(eventId);
        });

        public ValueTask<EventCallV1> RunEventCallAsync(EventCallV1 eventCall) =>
        TryCatch(async () =>
        {
            ValidateEventCallIsNotNull(eventCall);

            return await this.eventCallV1ProcessingService.RunEventCallAsync(
                eventCall);
        });

        public ValueTask<EventCallV1> RunEventCallAsyncV1(EventCallV1 eventCall) =>
        TryCatch(async () =>
        {
            ValidateEventCallIsNotNull(eventCall);

            return await this.eventCallV1ProcessingService.RunEventCallV1Async(
                eventCall);
        });
    }
}
