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
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService : IHealthV2CoordinationService
    {
        private readonly IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService;
        private readonly IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService;
        private readonly IHealthArchivedEventsV2OrchestrationService healthArchivedEventsV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public HealthV2CoordinationService(
            IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService,
            IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService,
            IHealthArchivedEventsV2OrchestrationService healthArchivedEventsV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.healthInfrastructureV2OrchestrationService = healthInfrastructureV2OrchestrationService;
            this.healthEventsV2OrchestrationService = healthEventsV2OrchestrationService;
            this.healthArchivedEventsV2OrchestrationService = healthArchivedEventsV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<HealthReportV2> RetrieveHealthCheckItemsReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveHealthReport(windowStart);

            HealthReportV2 infrastructurePartialReport =
                await this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 eventsPartialReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 archivedEventsPartialReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 report = await BuildReportShellAsync(period, windowStart);
            HealthConfiguration healthConfiguration = this.configurationBroker.GetHealthConfiguration();

            IReadOnlyList<HealthCheckItemV2> healthCheckItems = MergeHealthCheckItems(
                infrastructurePartialReport, eventsPartialReport, archivedEventsPartialReport);

            ScoreHealthCheckItems(healthCheckItems, healthConfiguration);
            report.HealthCheckItems = healthCheckItems;

            return report;
        });

        public ValueTask<HealthReportV2> RetrieveTrafficReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveHealthReport(windowStart);

            HealthReportV2 eventsPartialReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 archivedEventsPartialReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 report = await BuildReportShellAsync(period, windowStart);

            report.Traffic = MergeTraffic(
                period,
                windowStart,
                report.WindowEnd,
                report.WindowLabel,
                eventsPartialReport?.Traffic,
                archivedEventsPartialReport?.Traffic);

            return report;
        });

        private static TrafficSnapshotV2 MergeTraffic(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            string windowLabel,
            TrafficSnapshotV2 liveTraffic,
            TrafficSnapshotV2 archivedTraffic)
        {
            if (liveTraffic is null && archivedTraffic is null)
            {
                return null;
            }

            List<TrafficBucketV2> liveBuckets =
                liveTraffic?.Buckets?.ToList() ?? new List<TrafficBucketV2>();

            List<TrafficBucketV2> archivedBuckets =
                archivedTraffic?.Buckets?.ToList() ?? new List<TrafficBucketV2>();

            List<TrafficBucketV2> mergedBuckets =
                EnumerateBucketStarts(period, windowStart, windowEnd)
                    .Select(bucket =>
                    {
                        TrafficBucketV2 liveBucket = liveBuckets
                            .FirstOrDefault(candidate => candidate.PeriodStart == bucket.Start);

                        TrafficBucketV2 archivedBucket = archivedBuckets
                            .FirstOrDefault(candidate => candidate.PeriodStart == bucket.Start);

                        return new TrafficBucketV2
                        {
                            PeriodStart = bucket.Start,
                            Label = bucket.Label,
                            Events = (liveBucket?.Events ?? 0) + (archivedBucket?.Events ?? 0),

                            ImmediateEvents =
                                (liveBucket?.ImmediateEvents ?? 0) + (archivedBucket?.ImmediateEvents ?? 0),

                            ScheduledEvents =
                                (liveBucket?.ScheduledEvents ?? 0) + (archivedBucket?.ScheduledEvents ?? 0),

                            ListenerEvents =
                                (liveBucket?.ListenerEvents ?? 0) + (archivedBucket?.ListenerEvents ?? 0),

                            Success = (liveBucket?.Success ?? 0) + (archivedBucket?.Success ?? 0),
                            Errors = (liveBucket?.Errors ?? 0) + (archivedBucket?.Errors ?? 0),
                            Pending = (liveBucket?.Pending ?? 0) + (archivedBucket?.Pending ?? 0),
                            Replays = (liveBucket?.Replays ?? 0) + (archivedBucket?.Replays ?? 0)
                        };
                    })
                    .ToList();

            return new TrafficSnapshotV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                WindowLabel = windowLabel,
                TotalEvents = (liveTraffic?.TotalEvents ?? 0) + (archivedTraffic?.TotalEvents ?? 0),

                TotalListenerEvents =
                    (liveTraffic?.TotalListenerEvents ?? 0) + (archivedTraffic?.TotalListenerEvents ?? 0),

                TotalSuccess = (liveTraffic?.TotalSuccess ?? 0) + (archivedTraffic?.TotalSuccess ?? 0),
                TotalErrors = (liveTraffic?.TotalErrors ?? 0) + (archivedTraffic?.TotalErrors ?? 0),
                TotalPending = (liveTraffic?.TotalPending ?? 0) + (archivedTraffic?.TotalPending ?? 0),
                TotalReplays = (liveTraffic?.TotalReplays ?? 0) + (archivedTraffic?.TotalReplays ?? 0),
                Buckets = mergedBuckets
            };
        }

        public ValueTask<HealthReportV2> RetrieveAddressUsageReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveHealthReport(windowStart);

            HealthReportV2 infrastructurePartialReport =
                await this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 eventsPartialReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 archivedEventsPartialReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 report = await BuildReportShellAsync(period, windowStart);
            HealthConfiguration healthConfiguration = this.configurationBroker.GetHealthConfiguration();

            report.AddressUsage = MergeAddressUsage(
                infrastructurePartialReport?.AddressUsage,
                eventsPartialReport?.AddressUsage,
                archivedEventsPartialReport?.AddressUsage,
                healthConfiguration);

            return report;
        });

        public ValueTask<HealthReportV2> RetrieveParticipantUsageReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveHealthReport(windowStart);

            HealthReportV2 infrastructurePartialReport =
                await this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 eventsPartialReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 report = await BuildReportShellAsync(period, windowStart);
            HealthConfiguration healthConfiguration = this.configurationBroker.GetHealthConfiguration();

            report.ParticipantUsage = MergeParticipantUsage(
                infrastructurePartialReport?.ParticipantUsage,
                eventsPartialReport?.ParticipantUsage,
                infrastructurePartialReport?.AddressUsage,
                healthConfiguration);

            return report;
        });

        public ValueTask<HealthReportV2> RetrieveLoopDetectionReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        private static IReadOnlyList<ParticipantUsageV2> MergeParticipantUsage(
            IReadOnlyList<ParticipantUsageV2> nameRows,
            IReadOnlyList<ParticipantUsageV2> liveCountRows,
            IReadOnlyList<EventAddressUsageV2> addressNameRows,
            HealthConfiguration healthConfiguration)
        {
            List<ParticipantUsageV2> names = nameRows?.ToList() ?? new List<ParticipantUsageV2>();
            List<ParticipantUsageV2> liveCounts = liveCountRows?.ToList() ?? new List<ParticipantUsageV2>();

            if (names.Count == 0 && liveCounts.Count == 0)
            {
                return null;
            }

            List<EventAddressUsageV2> addressNames =
                addressNameRows?.ToList() ?? new List<EventAddressUsageV2>();

            long totalSent = liveCounts.Sum(row => row.TotalEventsSubmitted);
            long totalReceived = liveCounts.Sum(row => row.TotalListenerEvents);

            return names.Select(row => row.EventParticipantV2Id)
                .Union(liveCounts.Select(row => row.EventParticipantV2Id))
                .Select(eventParticipantV2Id =>
                {
                    ParticipantUsageV2 nameRow =
                        names.FirstOrDefault(row => row.EventParticipantV2Id == eventParticipantV2Id);

                    ParticipantUsageV2 liveRow =
                        liveCounts.FirstOrDefault(row => row.EventParticipantV2Id == eventParticipantV2Id);

                    List<ParticipantAddressUsageV2> byAddress = (liveRow?.ByAddress
                        ?? Enumerable.Empty<ParticipantAddressUsageV2>())
                        .Select(addressRow => new ParticipantAddressUsageV2
                        {
                            EventAddressV2Id = addressRow.EventAddressV2Id,

                            EventAddressV2Name = addressNames
                                .FirstOrDefault(addressName =>
                                    addressName.EventAddressV2Id == addressRow.EventAddressV2Id)?.Name,

                            Sent = addressRow.Sent,

                            SentPercentage = totalSent == 0
                                ? 0
                                : (decimal)addressRow.Sent * 100 / totalSent,

                            Received = addressRow.Received,

                            ReceivedPercentage = totalReceived == 0
                                ? 0
                                : (decimal)addressRow.Received * 100 / totalReceived
                        })
                        .ToList();

                    return new ParticipantUsageV2
                    {
                        EventParticipantV2Id = eventParticipantV2Id,
                        Name = nameRow?.Name ?? "Unknown",
                        ContactEmail = nameRow?.ContactEmail,
                        ContactPhone = nameRow?.ContactPhone,
                        IsActive = nameRow?.IsActive ?? false,
                        OwnedListeners = nameRow?.OwnedListeners ?? 0,
                        TotalEventsSubmitted = liveRow?.TotalEventsSubmitted ?? 0,
                        TotalListenerEvents = liveRow?.TotalListenerEvents ?? 0,
                        LoopsDetected = liveRow?.LoopsDetected ?? 0,
                        DuplicatesDetected = liveRow?.DuplicatesDetected ?? 0,
                        ByAddress = byAddress,

                        Status = ComputeRagStatus(
                            liveRow?.LoopsDetected ?? 0, HealthMetric.LoopsDetected, healthConfiguration)
                    };
                })
                .ToList();
        }

        private static IReadOnlyList<EventAddressUsageV2> MergeAddressUsage(
            IReadOnlyList<EventAddressUsageV2> nameRows,
            IReadOnlyList<EventAddressUsageV2> liveCountRows,
            IReadOnlyList<EventAddressUsageV2> archivedCountRows,
            HealthConfiguration healthConfiguration)
        {
            List<EventAddressUsageV2> names = nameRows?.ToList() ?? new List<EventAddressUsageV2>();
            List<EventAddressUsageV2> liveCounts = liveCountRows?.ToList() ?? new List<EventAddressUsageV2>();

            List<EventAddressUsageV2> archivedCounts =
                archivedCountRows?.ToList() ?? new List<EventAddressUsageV2>();

            if (names.Count == 0 && liveCounts.Count == 0 && archivedCounts.Count == 0)
            {
                return null;
            }

            return names.Select(row => row.EventAddressV2Id)
                .Union(liveCounts.Select(row => row.EventAddressV2Id))
                .Union(archivedCounts.Select(row => row.EventAddressV2Id))
                .Select(eventAddressV2Id =>
                {
                    EventAddressUsageV2 nameRow =
                        names.FirstOrDefault(row => row.EventAddressV2Id == eventAddressV2Id);

                    EventAddressUsageV2 liveRow =
                        liveCounts.FirstOrDefault(row => row.EventAddressV2Id == eventAddressV2Id);

                    EventAddressUsageV2 archivedRow =
                        archivedCounts.FirstOrDefault(row => row.EventAddressV2Id == eventAddressV2Id);

                    HealthStatusV2 deadStatus = ComputeRagStatus(
                        liveRow?.DeadEvents ?? 0, HealthMetric.DeadEvents, healthConfiguration);

                    HealthStatusV2 loopsStatus = ComputeRagStatus(
                        liveRow?.LoopsDetected ?? 0, HealthMetric.LoopsDetected, healthConfiguration);

                    return new EventAddressUsageV2
                    {
                        EventAddressV2Id = eventAddressV2Id,
                        Name = nameRow?.Name,
                        Description = nameRow?.Description,
                        ActiveListeners = nameRow?.ActiveListeners ?? 0,
                        TotalActiveEvents = liveRow?.TotalActiveEvents ?? 0,
                        TotalListenerEvents = liveRow?.TotalListenerEvents ?? 0,
                        DeadEvents = liveRow?.DeadEvents ?? 0,
                        LoopsDetected = liveRow?.LoopsDetected ?? 0,
                        TotalArchivedEvents = archivedRow?.TotalArchivedEvents ?? 0,
                        TotalArchivedListenerEvents = archivedRow?.TotalArchivedListenerEvents ?? 0,
                        Status = WorstOf(deadStatus, loopsStatus)
                    };
                })
                .ToList();
        }

        private static HealthStatusV2 WorstOf(params HealthStatusV2[] statuses)
        {
            if (statuses.Contains(HealthStatusV2.Red)) return HealthStatusV2.Red;
            if (statuses.Contains(HealthStatusV2.Amber)) return HealthStatusV2.Amber;
            if (statuses.Contains(HealthStatusV2.Green)) return HealthStatusV2.Green;

            return HealthStatusV2.NA;
        }

        private static List<(DateTimeOffset Start, string Label)> EnumerateBucketStarts(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd)
        {
            var bucketStarts = new List<(DateTimeOffset Start, string Label)>();

            switch (period)
            {
                case TrafficPeriodV2.Week:
                    for (int day = 0; day < 7; day++)
                    {
                        DateTimeOffset start = windowStart.AddDays(day);
                        bucketStarts.Add((start, start.ToString("ddd", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Month:
                    for (DateTimeOffset start = windowStart; start < windowEnd; start = start.AddDays(1))
                    {
                        bucketStarts.Add((start, start.ToString("dd", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Year:
                    for (int month = 0; month < 12; month++)
                    {
                        DateTimeOffset start = windowStart.AddMonths(month);
                        bucketStarts.Add((start, start.ToString("MMM", CultureInfo.InvariantCulture)));
                    }

                    break;

                default:
                    for (int hour = 0; hour < 24; hour++)
                    {
                        DateTimeOffset start = windowStart.AddHours(hour);
                        bucketStarts.Add((start, start.ToString("HH:00", CultureInfo.InvariantCulture)));
                    }

                    break;
            }

            return bucketStarts;
        }

        private async ValueTask<HealthReportV2> BuildReportShellAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart)
        {
            DateTimeOffset windowEnd = ComputeWindowEnd(period, windowStart);
            DateTimeOffset generatedDate = await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                WindowLabel = BuildWindowLabel(period, windowStart, windowEnd),
                GeneratedDate = generatedDate
            };
        }

        private static IReadOnlyList<HealthCheckItemV2> MergeHealthCheckItems(
            params HealthReportV2[] partialReports)
        {
            return partialReports
                .Where(partialReport => partialReport?.HealthCheckItems != null)
                .SelectMany(partialReport => partialReport.HealthCheckItems)
                .ToList();
        }

        private static void ScoreHealthCheckItems(
            IReadOnlyList<HealthCheckItemV2> healthCheckItems,
            HealthConfiguration healthConfiguration)
        {
            foreach (HealthCheckItemV2 healthCheckItem in healthCheckItems)
            {
                HealthMetric? metric = MapToHealthMetric(healthCheckItem.Grouping, healthCheckItem.Item);

                if (metric is null)
                {
                    continue;
                }

                decimal? metricValue = ParseMetricValue(healthCheckItem.Value);

                if (metricValue is null)
                {
                    continue;
                }

                HealthStatusV2 status =
                    ComputeRagStatus(metricValue.Value, metric.Value, healthConfiguration);

                healthCheckItem.StatusCode = (int)status;
                healthCheckItem.Status = status.ToString();
            }
        }

        private static HealthMetric? MapToHealthMetric(string grouping, string item)
        {
            switch (grouping, item)
            {
                case ("Infrastructure", "Registered Handlers"):
                    return HealthMetric.HandlerCount;

                case ("Active Events", "Total Quarantined"):
                case ("Active Events", "Loops Detected"):
                case ("Active Events", "Duplicates Blocked"):
                    return HealthMetric.LoopsDetected;

                case ("Active Listeners", "Total Error"):
                    return HealthMetric.ErrorRate;

                case ("Active Listeners", "Dead (No Retries)"):
                    return HealthMetric.DeadEvents;

                case ("Archived Listeners", "Total Error"):
                    return HealthMetric.ArchiveErrorRate;

                case ("Archived Listeners", "Dead (No Retries)"):
                    return HealthMetric.DeadArchivedEvents;

                default:
                    return null;
            }
        }

        private static decimal? ParseMetricValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int openParenthesisIndex = value.IndexOf('(');
            int percentIndex = value.IndexOf('%');

            string numericPart = openParenthesisIndex >= 0 && percentIndex > openParenthesisIndex
                ? value.Substring(openParenthesisIndex + 1, percentIndex - openParenthesisIndex - 1)
                : value;

            return decimal.TryParse(
                numericPart, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue)
                    ? parsedValue
                    : (decimal?)null;
        }

        private static HealthStatusV2 ComputeRagStatus(
            decimal value,
            HealthMetric metric,
            HealthConfiguration healthConfiguration)
        {
            RagThreshold threshold =
                healthConfiguration?.Thresholds?.FirstOrDefault(ragThreshold => ragThreshold.Metric == metric);

            if (threshold is null)
            {
                return HealthStatusV2.NA;
            }

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

        private static DateTimeOffset ComputeWindowEnd(TrafficPeriodV2 period, DateTimeOffset windowStart)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                    return windowStart.AddDays(7);

                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(windowStart.Year, windowStart.Month, 1, 0, 0, 0, TimeSpan.Zero)
                        .AddMonths(1);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(windowStart.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

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
