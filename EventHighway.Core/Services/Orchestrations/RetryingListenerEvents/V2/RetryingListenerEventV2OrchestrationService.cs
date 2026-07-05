// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2
{
    internal partial class RetryingListenerEventV2OrchestrationService
        : IRetryingListenerEventV2OrchestrationService
    {
        private readonly IEventCallV2ProcessingService eventCallV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public RetryingListenerEventV2OrchestrationService(
            IEventCallV2ProcessingService eventCallV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventCallV2ProcessingService = eventCallV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV2> RetryListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2IsNotNull(listenerEventV2);

            IEnumerable<string> requiredKeys =
                string.IsNullOrWhiteSpace(listenerEventV2.EventListenerV2.PromotedProperties)
                    ? Array.Empty<string>()
                    : await this.eventCallV2ProcessingService
                        .SplitPromotedPropertyKeysAsync(
                            listenerEventV2.EventListenerV2.PromotedProperties,
                            cancellationToken);

            var eventCallV2 = new EventCallV2
            {
                Content = listenerEventV2.EventV2.Content,
                HandlerId = listenerEventV2.EventListenerV2.HandlerId,
                HandlerName = listenerEventV2.EventListenerV2.HandlerName,
                FilterCriteria = listenerEventV2.EventListenerV2.FilterCriteria,
                RequiredPromotedProperties = requiredKeys,
                Response = null
            };

            bool isSuccess;

            try
            {
                eventCallV2.PromotedProperties =
                    string.IsNullOrWhiteSpace(listenerEventV2.EventV2.Content)
                        || string.IsNullOrWhiteSpace(listenerEventV2.EventListenerV2.PromotedProperties)
                    ? new List<PromotedProperty>()
                    : await this.eventCallV2ProcessingService
                        .PromotePropertiesAsync(
                            listenerEventV2.EventV2.Content,
                            listenerEventV2.EventListenerV2.PromotedProperties,
                            cancellationToken);

                EventCallV2 ranEventCallV2 =
                    await this.eventCallV2ProcessingService
                        .RunEventCallV2Async(eventCallV2, cancellationToken);

                listenerEventV2.Response = ranEventCallV2.Response;
                listenerEventV2.ResponseCode = ranEventCallV2.ResponseCode;
                listenerEventV2.ResponseMessage = ranEventCallV2.ResponseMessage;

                isSuccess = ranEventCallV2.IsSuccess;
            }
            catch (Exception exception)
            {
                await this.loggingBroker.LogErrorAsync(exception);
                listenerEventV2.Response = exception.Message;
                isSuccess = false;
            }

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            listenerEventV2.DispatchedDate = now;

            if (isSuccess)
            {
                listenerEventV2.Status = ListenerEventStatusV2.Success;
                listenerEventV2.NextRetryAttemptNotBefore = null;
            }
            else
            {
                listenerEventV2.RemainingRetryAttempts--;

                int attemptNumber =
                    listenerEventV2.RetryAttemptsAllowed
                        - listenerEventV2.RemainingRetryAttempts;

                RetryConfiguration retryConfiguration =
                    this.configurationBroker.GetRetryConfiguration();

                int backoffMinutes =
                    CalculateFibonacciBackoffMinutes(
                        attemptNumber,
                        retryConfiguration.RetryBackoffMaxMinutes);

                listenerEventV2.NextRetryAttemptNotBefore = now.AddMinutes(backoffMinutes);
                listenerEventV2.Status = ListenerEventStatusV2.Error;
            }

            listenerEventV2.UpdatedDate = now;

            return await this.listenerEventV2ProcessingService
                .ModifyListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask RetryFailedListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        private static int CalculateFibonacciBackoffMinutes(int attemptNumber, int maxMinutes)
        {
            if (attemptNumber <= 2)
            {
                return Math.Min(1, maxMinutes);
            }

            int previous = 1;
            int current = 1;

            for (int index = 3; index <= attemptNumber; index++)
            {
                int next = previous + current;

                if (next >= maxMinutes)
                {
                    return maxMinutes;
                }

                previous = current;
                current = next;
            }

            return current;
        }
    }
}
