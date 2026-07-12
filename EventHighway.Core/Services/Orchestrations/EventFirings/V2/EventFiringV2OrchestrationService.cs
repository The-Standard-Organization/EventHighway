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
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.EventFirings.V2
{
    internal partial class EventFiringV2OrchestrationService : IEventFiringV2OrchestrationService
    {
        private readonly IEventListenerV2ProcessingService eventListenerV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IEventCallV2ProcessingService eventCallV2ProcessingService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventFiringV2OrchestrationService(
            IEventListenerV2ProcessingService eventListenerV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IEventCallV2ProcessingService eventCallV2ProcessingService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventListenerV2ProcessingService = eventListenerV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.eventCallV2ProcessingService = eventCallV2ProcessingService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV2> FireEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2IsNotNull(eventV2);

            IQueryable<EventListenerV2> eventListenerV2s =
                await this.eventListenerV2ProcessingService
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventV2.EventAddressV2Id, cancellationToken);

            var processedListenerEventV2s = new List<ListenerEventV2>();

            foreach (EventListenerV2 eventListenerV2 in eventListenerV2s)
            {
                DateTimeOffset now =
                    await this.dateTimeBroker.GetDateTimeOffsetAsync();

                DateTimeOffset truncatedNow = 
                    TruncateToMicroseconds(now);

                ListenerEventV2 listenerEventV2 =
                    CreateListenerEventV2(
                        eventV2,
                        eventListenerV2,
                        truncatedNow);

                ListenerEventV2 addedListenerEventV2 =
                    await this.listenerEventV2ProcessingService
                        .AddListenerEventV2Async(listenerEventV2, cancellationToken);

                ListenerEventV2 processedListenerEventV2 =
                    await RunEventCallV2Async(
                        eventV2,
                        eventListenerV2,
                        addedListenerEventV2,
                        cancellationToken);

                processedListenerEventV2s.Add(processedListenerEventV2);
            }

            eventV2.ListenerEventV2s = processedListenerEventV2s;

            return eventV2;
        });

        private async Task<ListenerEventV2> RunEventCallV2Async(
            EventV2 eventV2,
            EventListenerV2 eventListenerV2,
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken)
        {
            IEnumerable<string> requiredKeys =
                string.IsNullOrWhiteSpace(eventListenerV2.PromotedProperties)
                    ? Array.Empty<string>()
                    : await this.eventCallV2ProcessingService
                        .SplitPromotedPropertyKeysAsync(
                            eventListenerV2.PromotedProperties,
                            cancellationToken);

            var eventCallV2 = new EventCallV2
            {
                Content = eventV2.Content,
                HandlerId = eventListenerV2.HandlerId,
                HandlerName = eventListenerV2.HandlerName,
                FilterCriteria = eventListenerV2.FilterCriteria,
                RequiredPromotedProperties = requiredKeys,
                Response = null
            };

            try
            {
                eventCallV2.PromotedProperties =
                    string.IsNullOrWhiteSpace(eventV2.Content)
                        || string.IsNullOrWhiteSpace(eventListenerV2.PromotedProperties)
                        ? new List<PromotedProperty>()
                        : await this.eventCallV2ProcessingService
                            .PromotePropertiesAsync(
                                eventV2.Content,
                                eventListenerV2.PromotedProperties,
                                cancellationToken);

                EventCallV2 ranEventCallV2 =
                    await this.eventCallV2ProcessingService
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

            return await this.listenerEventV2ProcessingService
                .ModifyListenerEventV2Async(listenerEventV2, cancellationToken);
        }

        private ListenerEventV2 CreateListenerEventV2(
            EventV2 eventV2,
            EventListenerV2 eventListenerV2,
            DateTimeOffset now)
        {
            RetryConfiguration retryConfiguration =
                this.configurationBroker.GetRetryConfiguration();

            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                EventV2Id = eventV2.Id,
                EventListenerV2Id = eventListenerV2.Id,
                EventAddressV2Id = eventV2.EventAddressV2Id,
                Status = ListenerEventStatusV2.Pending,
                RemainingRetryAttempts = retryConfiguration.RetryAttemptsAllowed,
                RetryAttemptsAllowed = retryConfiguration.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = null,
                DispatchedDate = now,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }

        private static DateTimeOffset TruncateToMicroseconds(
            DateTimeOffset dateTimeOffset)
        {
            long ticksToRemove = dateTimeOffset.Ticks % TimeSpan.TicksPerMicrosecond;

            return dateTimeOffset.AddTicks(-ticksToRemove);
        }
    }
}
