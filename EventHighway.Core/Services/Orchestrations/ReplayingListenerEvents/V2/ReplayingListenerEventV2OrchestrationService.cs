// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2
{
    internal partial class ReplayingListenerEventV2OrchestrationService
        : IReplayingListenerEventV2OrchestrationService
    {
        private readonly IEventCallV2ProcessingService eventCallV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReplayingListenerEventV2OrchestrationService(
            IEventCallV2ProcessingService eventCallV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventCallV2ProcessingService = eventCallV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfReplayListenerEventV2sAsync(
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            return await this.listenerEventV2ProcessingService
                .RetrieveBatchOfReplayListenerEventV2sAsync(take, cancellationToken);
        });

        public ValueTask<ListenerEventV2> ProcessReplayListenerEventV2Async(
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

                listenerEventV2.Status = ranEventCallV2.IsSuccess
                    ? ListenerEventStatusV2.Success
                    : ListenerEventStatusV2.Error;
            }
            catch (Exception exception)
            {
                await this.loggingBroker.LogErrorAsync(exception);
                listenerEventV2.Response = exception.Message;
                listenerEventV2.Status = ListenerEventStatusV2.Error;
            }

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            listenerEventV2.DispatchedDate = now;
            listenerEventV2.UpdatedDate = now;

            return await this.listenerEventV2ProcessingService
                .ModifyListenerEventV2Async(listenerEventV2, cancellationToken);
        });
    }
}
