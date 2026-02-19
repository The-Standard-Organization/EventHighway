// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using EventHighway.Core.Services.Orchestrations.Events.V1;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal partial class EventV1CoordinationService : IEventV1CoordinationService
    {
        private readonly IEventV1OrchestrationService eventV1OrchestrationService;
        private readonly IEventListenerV1OrchestrationService eventListenerV1OrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1CoordinationService(
            IEventV1OrchestrationService eventV1OrchestrationService,
            IEventListenerV1OrchestrationService eventListenerV1OrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV1OrchestrationService = eventV1OrchestrationService;
            this.eventListenerV1OrchestrationService = eventListenerV1OrchestrationService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1) =>
        TryCatch(returningEventV1Function: async () =>
        {
            ValidateEventV1IsNotNull(eventV1);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            eventV1.Type = eventV1.ScheduledDate switch
            {
                null => EventV1Type.Immediate,

                DateTimeOffset scheduledDate
                    when scheduledDate < now => EventV1Type.Immediate,

                _ => EventV1Type.Scheduled,
            };

            EventV1 submittedEventV1 =
                await this.eventV1OrchestrationService
                    .SubmitEventV1Async(eventV1);

            if (submittedEventV1.Type is EventV1Type.Immediate)
                await ProcessEventListenerV1sAsync(submittedEventV1);

            return submittedEventV1;
        },

        retryEventV1Function: async () =>
        {
            if (eventV1.RetryAttempts > 0)
            {
                eventV1.RetryAttempts--;
                return await SubmitEventV1Async(eventV1);
            }

            return null;
        } // retry logic
    );

        public ValueTask FireScheduledPendingEventV1sAsync() =>
        TryCatch(async () =>
        {
            IQueryable<EventV1> eventV1s =
                await this.eventV1OrchestrationService
                    .RetrieveScheduledPendingEventV1sAsync();

            foreach (EventV1 eventV1 in eventV1s)
            {
                await ProcessEventListenerV1sAsync(eventV1);

                await this.eventV1OrchestrationService
                    .MarkEventV1AsImmediateAsync(eventV1);
            }
        });

        private async ValueTask ProcessEventListenerV1sAsync(EventV1 eventV1)
        {
            IQueryable<EventListenerV1> eventListenerV1s =
                await this.eventListenerV1OrchestrationService
                    .RetrieveEventListenerV1sByEventAddressIdAsync(
                        eventV1.EventAddressId);

            foreach (EventListenerV1 eventListenerV1 in eventListenerV1s)
            {
                DateTimeOffset now =
                    await this.dateTimeBroker.GetDateTimeOffsetAsync();

                ListenerEventV1 listenerEventV1 =
                    CreateEventListenerV1(
                        eventV1,
                        eventListenerV1,
                        now);

                ListenerEventV1 addedListenerEventV1 =
                    await this.eventListenerV1OrchestrationService
                        .AddListenerEventV1Async(listenerEventV1);

                await RunEventCallV1Async(
                    eventV1,
                    eventListenerV1,
                    addedListenerEventV1);
            }
        }

        public ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventV1Id(eventV1Id);

            return await this.eventV1OrchestrationService
                .RemoveEventV1ByIdAsync(eventV1Id);
        });

        private async Task RunEventCallV1Async(
            EventV1 eventV1,
            EventListenerV1 eventListenerV1,
            ListenerEventV1 listenerEventV1)
        {
            var eventCallV1 = new EventCallV1
            {
                Content = eventV1.Content,
                Endpoint = eventListenerV1.Endpoint,
                Secret = eventListenerV1.HeaderSecret,
                Response = null
            };

            try
            {
                EventCallV1 ranEventCallV1 =
                    await this.eventV1OrchestrationService
                        .RunEventCallV1Async(eventCallV1);

                listenerEventV1.Response = ranEventCallV1.Response;
                listenerEventV1.Status = ListenerEventV1Status.Success;
            }
            catch (Exception exception)
            {
                listenerEventV1.Response = exception.Message;
                listenerEventV1.Status = ListenerEventV1Status.Error;
            }

            eventV1.ListenerEvents.Add(listenerEventV1); // fire and observe

            listenerEventV1.UpdatedDate =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            await this.eventListenerV1OrchestrationService
                .ModifyListenerEventV1Async(listenerEventV1);
        }

        private static ListenerEventV1 CreateEventListenerV1(
            EventV1 eventV1,
            EventListenerV1 eventListenerV1,
            DateTimeOffset now)
        {
            return new ListenerEventV1
            {
                Id = Guid.NewGuid(),
                EventId = eventV1.Id,
                EventListenerId = eventListenerV1.Id,
                EventAddressId = eventV1.EventAddressId,
                Status = ListenerEventV1Status.Pending,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }
    }
}
