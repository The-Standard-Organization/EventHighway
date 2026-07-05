// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.AddressSummaries.V2
{
    internal partial class AddressSummaryV2OrchestrationService : IAddressSummaryV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public AddressSummaryV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            IEventArchiveV2Service eventArchiveV2Service,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<EventAddressSummaryV2>> RetrieveEventAddressSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset effectiveWindowStart = ResolveWindowStart(windowStart);
            DateTimeOffset windowEnd = ComputeWindowEnd(period, effectiveWindowStart);
            string windowLabel = BuildWindowLabel(period, effectiveWindowStart, windowEnd);

            var allAddresses =
                (await this.eventAddressV2Service
                    .RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                        .ToList();

            var allEvents =
                (await this.eventV2Service
                    .RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                        .ToList();

            var allArchivedEvents =
                (await this.eventArchiveV2Service
                    .RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken))
                        .ToList();

            var allListeners = allAddresses
                .SelectMany(address =>
                    address.EventListenerV2s ?? Enumerable.Empty<EventListenerV2>())
                .ToList();

            var allListenerEvents = allEvents
                .SelectMany(eventV2 =>
                    eventV2.ListenerEventV2s ?? Enumerable.Empty<ListenerEventV2>())
                .ToList();

            var allArchivedListenerEvents = allArchivedEvents
                .SelectMany(archive =>
                    archive.ListenerEventArchiveV2s ?? Enumerable.Empty<ListenerEventArchiveV2>())
                .ToList();

            HealthConfiguration healthConfig =
                this.configurationBroker.GetHealthConfiguration();

            var summaries = new List<EventAddressSummaryV2>();

            foreach (var address in allAddresses)
            {
                var addressEvents = allEvents
                    .Where(e => e.EventAddressV2Id == address.Id
                        && e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                    .ToList();

                var addressListenerEvents = allListenerEvents
                    .Where(le => le.EventAddressV2Id == address.Id
                        && le.CreatedDate >= effectiveWindowStart && le.CreatedDate < windowEnd)
                    .ToList();

                int totalArchivedEvents = allArchivedEvents.Count(a =>
                    a.EventAddressV2Id == address.Id
                    && a.ArchivedDate >= effectiveWindowStart && a.ArchivedDate < windowEnd);

                int totalArchivedListenerEvents = allArchivedListenerEvents.Count(la =>
                    la.EventAddressV2Id == address.Id
                    && la.ArchivedDate >= effectiveWindowStart && la.ArchivedDate < windowEnd);

                int activeListeners = allListeners.Count(l => l.EventAddressV2Id == address.Id);

                int totalListenerEvents = addressListenerEvents.Count;
                int errorListenerEvents = addressListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error);

                decimal errorRate = totalListenerEvents > 0
                    ? (decimal)errorListenerEvents / totalListenerEvents * 100
                    : 0;

                int totalActiveEvents = addressEvents.Count;
                int distinctContentHashes = addressEvents.Select(e => e.ContentHash).Distinct().Count();
                int duplicates = totalActiveEvents - distinctContentHashes;

                decimal duplicateRate = totalActiveEvents > 0
                    ? (decimal)duplicates / totalActiveEvents * 100
                    : 0;

                var activityDates = addressEvents.Select(e => e.CreatedDate)
                    .Concat(addressListenerEvents.Select(le => le.CreatedDate))
                    .ToList();

                DateTimeOffset? lastActivity = activityDates.Count > 0
                    ? activityDates.Max()
                    : (DateTimeOffset?)null;

                summaries.Add(new EventAddressSummaryV2
                {
                    EventAddressV2Id = address.Id,
                    Name = address.Name,
                    Description = address.Description,
                    Period = period,
                    WindowStart = effectiveWindowStart,
                    WindowEnd = windowEnd,
                    WindowLabel = windowLabel,
                    TotalActiveEvents = totalActiveEvents,
                    TotalArchivedEvents = totalArchivedEvents,
                    TotalListenerEvents = totalListenerEvents,
                    TotalArchivedListenerEvents = totalArchivedListenerEvents,
                    ActiveListeners = activeListeners,

                    // TODO: retry health rework (listener-level) - event-level RemainingRetryAttempts removed; zeroed until the Health overhaul.
                    DeadEvents = 0,
                    LoopsDetected = addressEvents.Count(e => e.Status == EventStatusV2.Quarantined),
                    ErrorRate = errorRate,
                    DuplicateRate = duplicateRate,
                    Status = ComputeRagStatus(errorRate, HealthMetric.ErrorRate, healthConfig),
                    LastActivity = lastActivity
                });
            }

            return summaries;
        });

        private static HealthStatusV2 ComputeRagStatus(
            decimal value,
            HealthMetric metric,
            HealthConfiguration healthConfig)
        {
            RagThreshold threshold =
                healthConfig.Thresholds.FirstOrDefault(t => t.Metric == metric);

            if (threshold is null)
                return HealthStatusV2.NA;

            if (threshold.Green < threshold.Red)
            {
                if (value <= threshold.Green) return HealthStatusV2.Green;
                if (value >= threshold.Red) return HealthStatusV2.Red;
                return HealthStatusV2.Amber;
            }

            if (threshold.Green > threshold.Red)
            {
                if (value >= threshold.Green) return HealthStatusV2.Green;
                if (value <= threshold.Red) return HealthStatusV2.Red;
                return HealthStatusV2.Amber;
            }

            return HealthStatusV2.NA;
        }

        private static DateTimeOffset ResolveWindowStart(DateTimeOffset windowStart) =>
            windowStart == DateTimeOffset.MinValue
                ? DateTimeOffset.UtcNow
                : windowStart;

        private static DateTimeOffset ComputeWindowEnd(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart)
        {
            switch (period)
            {
                case TrafficPeriodV2.Day:
                    return windowStart.AddHours(24);

                case TrafficPeriodV2.Week:
                    return windowStart.AddDays(7);

                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(
                        windowStart.Year, windowStart.Month, 1, 0, 0, 0, TimeSpan.Zero)
                            .AddMonths(1);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(
                        windowStart.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

                default:
                    return windowStart.AddHours(24);
            }
        }

        private static string BuildWindowLabel(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd)
        {
            switch (period)
            {
                case TrafficPeriodV2.Day:
                    return windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

                case TrafficPeriodV2.Week:
                    return $"{windowStart.ToString("dd MMM", CultureInfo.InvariantCulture)} – " +
                        $"{windowEnd.AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";

                case TrafficPeriodV2.Month:
                    return windowStart.ToString("MMM yyyy", CultureInfo.InvariantCulture);

                case TrafficPeriodV2.Year:
                    return windowStart.Year.ToString(CultureInfo.InvariantCulture);

                default:
                    return windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            }
        }
    }
}
