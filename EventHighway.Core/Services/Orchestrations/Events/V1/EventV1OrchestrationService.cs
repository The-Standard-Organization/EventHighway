// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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

        public ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1) =>
        TryCatch(async () =>
        {
            ValidateEventV1IsNotNull(eventV1);

            EventAddressV1 maybeEventAddressV1 =
                await this.eventAddressV1ProcessingService
                    .RetrieveEventAddressV1ByIdAsync(
                        eventV1.EventAddressId);

            ValidateListenerEventV1Exists(
                maybeEventAddressV1,
                eventV1.EventAddressId);

            return await this.eventV1ProcessingService
                .AddEventV1Async(eventV1);
        });

        public ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventV1sAsync() =>
        TryCatch(async () =>
        {
            return await this.eventV1ProcessingService
                .RetrieveScheduledPendingEventV1sAsync();
        });

        public ValueTask<EventV1> MarkEventV1AsImmediateAsync(EventV1 eventV1) =>
        TryCatch(async () =>
        {
            ValidateEventV1IsNotNull(eventV1);

            return await this.eventV1ProcessingService
                .MarkEventV1AsImmediateAsync(eventV1);
        });

        public ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventV1Id(eventV1Id);

            return await this.eventV1ProcessingService
                .RemoveEventV1ByIdAsync(eventV1Id);
        });

        public ValueTask<EventCallV1> RunEventCallV1Async(EventCallV1 eventCallV1) =>
        TryCatch(async () =>
        {
            ValidateEventCallV1IsNotNull(eventCallV1);

            return await this.eventCallV1ProcessingService.RunEventCallV1Async(
                eventCallV1);
        });

        public ValueTask<EventCallV1> RunEventCallV1AsyncV1(EventCallV1 eventCallV1) =>
        TryCatch(async () =>
        {
            ValidateEventCallV1IsNotNull(eventCallV1);

            return await this.eventCallV1ProcessingService.RunEventCallV1AsyncV1(
                eventCallV1);
        });
    }
}
