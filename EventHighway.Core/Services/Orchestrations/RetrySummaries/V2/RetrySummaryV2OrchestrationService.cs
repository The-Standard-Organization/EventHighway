// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.RetrySummaries.V2
{
    internal partial class RetrySummaryV2OrchestrationService : IRetrySummaryV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public RetrySummaryV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allAddresses =
                (await this.eventAddressV2Service
                    .RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                        .ToList();

            var allEvents =
                (await this.eventV2Service
                    .RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                        .ToList();

            var activeEvents = allEvents
                .Where(e => e.Status == EventStatusV2.Active)
                .ToList();

            var addressNames = allAddresses
                .ToDictionary(address => address.Id, address => address.Name);

            // TODO: retry health rework (listener-level) - event-level RemainingRetryAttempts removed;
            // retry figures are zeroed here until the Health overhaul rebuilds them from ListenerEventV2.
            var byAddress = activeEvents
                .GroupBy(e => e.EventAddressV2Id)
                .Select(group => new RetryAddressDetailV2
                {
                    EventAddressV2Id = group.Key,
                    EventAddressV2Name = addressNames.TryGetValue(group.Key, out string name) ? name : null,
                    DeadEvents = 0,
                    CriticalEvents = 0,
                    TotalEvents = group.Count()
                })
                .OrderByDescending(detail => detail.TotalEvents)
                .ToList();

            return new RetryHealthSummaryV2
            {
                TotalActiveEvents = activeEvents.Count,
                DeadEvents = 0,
                CriticalEvents = 0,
                HealthyEvents = 0,
                Distribution = new List<RetryBucketV2>(),
                ByAddress = byAddress
            };
        });
    }
}
