// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;

namespace EventHighway.Core.Services.Coordinations.Events.V2
{
    internal partial class EventV2CoordinationService : IEventV2CoordinationService
    {
        private readonly IEventV2OrchestrationService eventV2OrchestrationService;
        private readonly IEventListenerV2OrchestrationService eventListenerV2OrchestrationService;
        private readonly IJsonBroker jsonBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2CoordinationService(
            IEventV2OrchestrationService eventV2OrchestrationService,
            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService,
            IJsonBroker jsonBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2OrchestrationService = eventV2OrchestrationService;
            this.eventListenerV2OrchestrationService = eventListenerV2OrchestrationService;
            this.jsonBroker = jsonBroker;
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

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            eventV2.Type = eventV2.ScheduledDate switch
            {
                null => EventTypeV2.Immediate,

                DateTimeOffset scheduledDate
                    when scheduledDate < now => EventTypeV2.Immediate,

                _ => EventTypeV2.Scheduled,
            };

            EventV2 submittedEventV2 =
                await this.eventV2OrchestrationService
                    .SubmitEventV2Async(eventV2, cancellationToken);

            if (submittedEventV2.Type is EventTypeV2.Immediate)
                await ProcessEventListenerV2sAsync(submittedEventV2, cancellationToken);

            return submittedEventV2;
        });

        public ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            IQueryable<EventV2> eventV2s =
                await this.eventV2OrchestrationService
                    .RetrieveScheduledPendingEventV2sAsync(cancellationToken);

            foreach (EventV2 eventV2 in eventV2s)
            {
                await ProcessEventListenerV2sAsync(eventV2, cancellationToken);

                await this.eventV2OrchestrationService
                    .MarkEventV2AsImmediateAsync(eventV2, cancellationToken);
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

        private async ValueTask ProcessEventListenerV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken)
        {
            IQueryable<EventListenerV2> eventListenerV2s =
                await this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventV2.EventAddressId, cancellationToken);

            foreach (EventListenerV2 eventListenerV2 in eventListenerV2s)
            {
                DateTimeOffset now =
                    await this.dateTimeBroker.GetDateTimeOffsetAsync();

                ListenerEventV2 listenerEventV2 =
                    CreateListenerEventV2(
                        eventV2,
                        eventListenerV2,
                        now);

                ListenerEventV2 addedListenerEventV2 =
                    await this.eventListenerV2OrchestrationService
                        .AddListenerEventV2Async(listenerEventV2, cancellationToken);

                await RunEventCallV2Async(
                    eventV2,
                    eventListenerV2,
                    addedListenerEventV2,
                    cancellationToken);
            }
        }

        private async Task RunEventCallV2Async(
            EventV2 eventV2,
            EventListenerV2 eventListenerV2,
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken)
        {
            var eventCallV2 = new EventCallV2
            {
                Content = eventV2.Content,
                HandlerId = eventListenerV2.HandlerId,
                HandlerName = eventListenerV2.HandlerName,
                FilterCriteria = eventListenerV2.FilterCriteria,
                RequiredPromotedProperties = SplitPromotedPropertyKeys(eventListenerV2.PromotedProperties),
                Response = null
            };

            try
            {
                eventCallV2.PromotedProperties = PromoteProperties(
                    content: eventV2.Content,
                    promotedProperties: eventListenerV2.PromotedProperties);

                EventCallV2 ranEventCallV2 =
                    await this.eventV2OrchestrationService
                        .RunEventCallV2Async(eventCallV2, cancellationToken);

                listenerEventV2.Response = ranEventCallV2.Response;
                listenerEventV2.ResponseCode = ranEventCallV2.ResponseCode;
                listenerEventV2.ResponseMessage = ranEventCallV2.ResponseMessage;

                listenerEventV2.Status = ranEventCallV2.IsSuccess
                    ? ListenerEventStatusV2.Success
                    : ListenerEventStatusV2.Error;
            }
            catch (Exception exception)
            {
                listenerEventV2.Response = exception.Message;
                listenerEventV2.Status = ListenerEventStatusV2.Error;
            }

            listenerEventV2.UpdatedDate =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            await this.eventListenerV2OrchestrationService
                .ModifyListenerEventV2Async(listenerEventV2, cancellationToken);
        }

        private static IEnumerable<string> SplitPromotedPropertyKeys(string promotedProperties) =>
            string.IsNullOrWhiteSpace(promotedProperties)
                ? Array.Empty<string>()
                : promotedProperties.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private List<PromotedProperty> PromoteProperties(
            string content,
            string promotedProperties)
        {
            if (string.IsNullOrWhiteSpace(promotedProperties) || string.IsNullOrWhiteSpace(content))
                return new List<PromotedProperty>();

            IEnumerable<string> keys = SplitPromotedPropertyKeys(promotedProperties);
            var result = new List<PromotedProperty>();

            try
            {
                foreach (string key in keys)
                {
                    if (this.jsonBroker.CheckIfPropertyExist(content, key))
                    {
                        result.Add(new PromotedProperty
                        {
                            Name = key,
                            Value = this.jsonBroker.GetJsonPropertyValue(content, key)
                        });
                    }
                }
            }
            catch
            {
                return new List<PromotedProperty>();
            }

            return result;
        }

        private static ListenerEventV2 CreateListenerEventV2(
            EventV2 eventV2,
            EventListenerV2 eventListenerV2,
            DateTimeOffset now)
        {
            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                EventId = eventV2.Id,
                EventListenerId = eventListenerV2.Id,
                EventAddressId = eventV2.EventAddressId,
                Status = ListenerEventStatusV2.Pending,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }
    }
}
