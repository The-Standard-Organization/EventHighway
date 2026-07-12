// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2
{
    internal partial class HealthArchivedEventsV2OrchestrationService : IHealthArchivedEventsV2OrchestrationService
    {
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;
        private readonly ILoggingBroker loggingBroker;

        public HealthArchivedEventsV2OrchestrationService(
            IEventArchiveV2Service eventArchiveV2Service,
            IListenerEventArchiveV2Service listenerEventArchiveV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<HealthReportV2> RetrieveHealthReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventArchiveV2> archivedEvents =
                await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync(cancellationToken);

            IQueryable<ListenerEventArchiveV2> archivedListenerEvents =
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);

            DateTimeOffset resolvedWindowEnd = windowEnd ?? ComputeWindowEnd(period, windowStart);

            IQueryable<EventArchiveV2> windowEvents = archivedEvents
                .Where(archivedEvent => archivedEvent.ArchivedDate >= windowStart
                    && archivedEvent.ArchivedDate < resolvedWindowEnd);

            IQueryable<ListenerEventArchiveV2> windowListenerEvents = archivedListenerEvents
                .Where(listenerEvent => listenerEvent.ArchivedDate >= windowStart
                    && listenerEvent.ArchivedDate < resolvedWindowEnd);

            long totalEvents = archivedEvents.LongCount();

            long totalQuarantined = archivedEvents
                .LongCount(archivedEvent => archivedEvent.Status == EventArchiveStatusV2.Quarantined);

            long totalListenerEvents = archivedListenerEvents.LongCount();

            long totalSuccess = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Success);

            long totalError = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error);

            long totalActiveRetries = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,

                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Total Events",
                        value: totalEvents,
                        description: "Total number of archived events."),

                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Total Quarantined",
                        value: totalQuarantined,
                        description: "Total number of quarantined archived events."),

                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Loops Detected",
                        value: totalQuarantined,
                        description: "Number of archived events quarantined by loop detection."),

                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Duplicates Blocked",
                        value: totalQuarantined,
                        description: "Number of archived events blocked as duplicates."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Total Listener Events",
                        value: totalListenerEvents,
                        description: "Total number of archived listener events."),

                    MapToRateHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Total Success",
                        count: totalSuccess,
                        total: totalListenerEvents,
                        description: "Archived listener events that completed successfully."),

                    MapToRateHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Total Errors",
                        count: totalError,
                        total: totalListenerEvents,
                        description: "Archived listener events that ended in an error state."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Active (Items With Retries Left)",
                        value: totalActiveRetries,
                        description: "Errored archived listener events with retry attempts remaining."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Dead (Items With No Retries)",
                        value: totalDead,
                        description: "Errored archived listener events with no retry attempts remaining.")
                },

                Traffic = MapToTrafficSnapshot(
                    period, windowStart, resolvedWindowEnd, windowEvents, windowListenerEvents),

                AddressUsage = MapToAddressUsage(windowEvents, windowListenerEvents),

                LoopDetection = MapToLoopDetection(period, windowStart, resolvedWindowEnd, windowEvents),

                Retry = MapToRetry(period, windowStart, resolvedWindowEnd, windowListenerEvents)
            };
        });

        private static HealthCheckItemV2 MapToHealthCheckItem(
            string grouping,
            string item,
            long value,
            string description)
        {
            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value.ToString(CultureInfo.InvariantCulture),
                Description = description,
                StatusCode = (int)HealthStatusV2.NA,
                Status = nameof(HealthStatusV2.NA)
            };
        }

        private static HealthCheckItemV2 MapToRateHealthCheckItem(
            string grouping,
            string item,
            long count,
            long total,
            string description)
        {
            decimal rate = total == 0
                ? 0
                : (decimal)count * 100 / total;

            string value = $"{count.ToString(CultureInfo.InvariantCulture)} " +
                $"({rate.ToString("0.00", CultureInfo.InvariantCulture)}%)";

            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value,
                Description = description,
                StatusCode = (int)HealthStatusV2.NA,
                Status = nameof(HealthStatusV2.NA)
            };
        }

        private static TrafficSnapshotV2 MapToTrafficSnapshot(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            IQueryable<EventArchiveV2> windowEvents,
            IQueryable<ListenerEventArchiveV2> windowListenerEvents)
        {
            return new TrafficSnapshotV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalEvents = windowEvents.LongCount(),
                TotalListenerEvents = windowListenerEvents.LongCount(),

                TotalSuccess = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Success),

                TotalErrors = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error),

                TotalPending = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Pending),

                TotalReplays = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Replay),

                Buckets = MapToTrafficBuckets(period, windowEvents, windowListenerEvents)
            };
        }

        private static IEnumerable<TrafficBucketV2> MapToTrafficBuckets(
            TrafficPeriodV2 period,
            IQueryable<EventArchiveV2> windowEvents,
            IQueryable<ListenerEventArchiveV2> windowListenerEvents)
        {
            // Bucket server-side by translatable date-parts then re-fold into period buckets in
            // memory (see HealthEvents for the rationale); ArchivedDate is stored UTC.
            var eventBuckets = windowEvents
                .GroupBy(archivedEvent => new
                {
                    archivedEvent.ArchivedDate.Year,
                    archivedEvent.ArchivedDate.Month,
                    archivedEvent.ArchivedDate.Day,
                    archivedEvent.ArchivedDate.Hour
                })
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Month,
                    group.Key.Day,
                    group.Key.Hour,
                    Events = group.LongCount(),
                    ImmediateEvents = group.LongCount(
                        archivedEvent => archivedEvent.Type == EventArchiveTypeV2.Immediate),
                    ScheduledEvents = group.LongCount(
                        archivedEvent => archivedEvent.Type == EventArchiveTypeV2.Scheduled)
                })
                .ToList()
                .GroupBy(bucket => MapToBucketStart(
                    period,
                    new DateTimeOffset(bucket.Year, bucket.Month, bucket.Day, bucket.Hour, 0, 0, TimeSpan.Zero)))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    Events = group.Sum(bucket => bucket.Events),
                    ImmediateEvents = group.Sum(bucket => bucket.ImmediateEvents),
                    ScheduledEvents = group.Sum(bucket => bucket.ScheduledEvents)
                })
                .ToList();

            var listenerBuckets = windowListenerEvents
                .GroupBy(listenerEvent => new
                {
                    listenerEvent.ArchivedDate.Year,
                    listenerEvent.ArchivedDate.Month,
                    listenerEvent.ArchivedDate.Day,
                    listenerEvent.ArchivedDate.Hour
                })
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Month,
                    group.Key.Day,
                    group.Key.Hour,
                    ListenerEvents = group.LongCount(),
                    Success = group.LongCount(
                        listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Success),
                    Errors = group.LongCount(
                        listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error),
                    Pending = group.LongCount(
                        listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Pending),
                    Replays = group.LongCount(
                        listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Replay)
                })
                .ToList()
                .GroupBy(bucket => MapToBucketStart(
                    period,
                    new DateTimeOffset(bucket.Year, bucket.Month, bucket.Day, bucket.Hour, 0, 0, TimeSpan.Zero)))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    ListenerEvents = group.Sum(bucket => bucket.ListenerEvents),
                    Success = group.Sum(bucket => bucket.Success),
                    Errors = group.Sum(bucket => bucket.Errors),
                    Pending = group.Sum(bucket => bucket.Pending),
                    Replays = group.Sum(bucket => bucket.Replays)
                })
                .ToList();

            return eventBuckets.Select(bucket => bucket.PeriodStart)
                .Union(listenerBuckets.Select(bucket => bucket.PeriodStart))
                .OrderBy(periodStart => periodStart)
                .Select(periodStart =>
                {
                    var eventBucket = eventBuckets.FirstOrDefault(bucket => bucket.PeriodStart == periodStart);

                    var listenerBucket =
                        listenerBuckets.FirstOrDefault(bucket => bucket.PeriodStart == periodStart);

                    return new TrafficBucketV2
                    {
                        PeriodStart = periodStart,
                        Events = eventBucket?.Events ?? 0,
                        ImmediateEvents = eventBucket?.ImmediateEvents ?? 0,
                        ScheduledEvents = eventBucket?.ScheduledEvents ?? 0,
                        ListenerEvents = listenerBucket?.ListenerEvents ?? 0,
                        Success = listenerBucket?.Success ?? 0,
                        Errors = listenerBucket?.Errors ?? 0,
                        Pending = listenerBucket?.Pending ?? 0,
                        Replays = listenerBucket?.Replays ?? 0
                    };
                })
                .ToList();
        }

        // Rolling windows anchored to the caller's start (see HealthV2CoordinationService for rationale).
        private static DateTimeOffset ComputeWindowEnd(TrafficPeriodV2 period, DateTimeOffset windowStart)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                    return windowStart.AddDays(7);

                case TrafficPeriodV2.Month:
                    return windowStart.AddMonths(1);

                case TrafficPeriodV2.Year:
                    return windowStart.AddYears(1);

                default:
                    return windowStart.AddHours(24);
            }
        }

        private static DateTimeOffset MapToBucketStart(TrafficPeriodV2 period, DateTimeOffset archivedDate)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(
                        archivedDate.Year, archivedDate.Month, archivedDate.Day, 0, 0, 0, TimeSpan.Zero);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(
                        archivedDate.Year, archivedDate.Month, 1, 0, 0, 0, TimeSpan.Zero);

                default:
                    return new DateTimeOffset(
                        archivedDate.Year, archivedDate.Month, archivedDate.Day, archivedDate.Hour, 0, 0,
                        TimeSpan.Zero);
            }
        }

        private static IReadOnlyList<EventAddressUsageV2> MapToAddressUsage(
            IQueryable<EventArchiveV2> windowEvents,
            IQueryable<ListenerEventArchiveV2> windowListenerEvents)
        {
            var eventCounts = windowEvents
                .GroupBy(archivedEvent => archivedEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalArchivedEvents = group.LongCount(),
                    LastActivity = group.Max(archivedEvent => archivedEvent.CreatedDate)
                })
                .ToList();

            var listenerCounts = windowListenerEvents
                .GroupBy(listenerEvent => listenerEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalArchivedListenerEvents = group.LongCount(),

                    ErrorListenerEvents = group.LongCount(listenerEvent =>
                        listenerEvent.Status == ListenerEventArchiveStatusV2.Error),

                    LastActivity = group.Max(listenerEvent => listenerEvent.CreatedDate)
                })
                .ToList();

            return eventCounts.Select(count => count.EventAddressV2Id)
                .Union(listenerCounts.Select(count => count.EventAddressV2Id))
                .OrderBy(eventAddressV2Id => eventAddressV2Id)
                .Select(eventAddressV2Id =>
                {
                    var eventCount =
                        eventCounts.FirstOrDefault(count => count.EventAddressV2Id == eventAddressV2Id);

                    var listenerCount =
                        listenerCounts.FirstOrDefault(count => count.EventAddressV2Id == eventAddressV2Id);

                    return new EventAddressUsageV2
                    {
                        EventAddressV2Id = eventAddressV2Id,
                        TotalArchivedEvents = eventCount?.TotalArchivedEvents ?? 0,
                        TotalArchivedListenerEvents = listenerCount?.TotalArchivedListenerEvents ?? 0,
                        ErrorListenerEvents = listenerCount?.ErrorListenerEvents ?? 0,

                        LastActivity = new[] { eventCount?.LastActivity, listenerCount?.LastActivity }
                            .Where(lastActivity => lastActivity is not null)
                            .Max()
                    };
                })
                .ToList();
        }

        private static LoopDetectionSummaryV2 MapToLoopDetection(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            IQueryable<EventArchiveV2> windowEvents)
        {
            IQueryable<EventArchiveV2> quarantinedEvents = windowEvents
                .Where(archivedEvent => archivedEvent.Status == EventArchiveStatusV2.Quarantined);

            long totalArchivedQuarantined = quarantinedEvents.LongCount();

            List<LoopDetailV2> byAddress = quarantinedEvents
                .GroupBy(archivedEvent => new { archivedEvent.EventAddressV2Id, archivedEvent.EventParticipantV2Id })
                .Select(group => new LoopDetailV2
                {
                    EventAddressV2Id = group.Key.EventAddressV2Id,
                    EventParticipantV2Id = group.Key.EventParticipantV2Id,
                    ArchivedQuarantined = group.LongCount(),
                    InWindow = group.LongCount(),
                    MostRecentDetection = group.Max(archivedEvent => archivedEvent.ArchivedDate)
                })
                .ToList();

            return new LoopDetectionSummaryV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalArchivedQuarantined = totalArchivedQuarantined,
                TotalInWindow = totalArchivedQuarantined,
                ByAddress = byAddress
            };
        }

        private static RetryHealthSummaryV2 MapToRetry(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            IQueryable<ListenerEventArchiveV2> windowListenerEvents)
        {
            IQueryable<ListenerEventArchiveV2> errorEvents = windowListenerEvents
                .Where(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error);

            List<RetryBucketV2> distribution = errorEvents
                .GroupBy(listenerEvent => listenerEvent.RemainingRetryAttempts)
                .Select(group => new RetryBucketV2
                {
                    RemainingRetries = group.Key,
                    Count = group.LongCount()
                })
                .OrderBy(bucket => bucket.RemainingRetries)
                .ToList();

            List<RetryAddressDetailV2> byAddress = errorEvents
                .GroupBy(listenerEvent => listenerEvent.EventAddressV2Id)
                .Select(group => new RetryAddressDetailV2
                {
                    EventAddressV2Id = group.Key,
                    DeadEvents = group.LongCount(listenerEvent => listenerEvent.RemainingRetryAttempts == 0),

                    CriticalEvents = group.LongCount(listenerEvent =>
                        listenerEvent.RemainingRetryAttempts == 1 || listenerEvent.RemainingRetryAttempts == 2),

                    HealthyEvents = group.LongCount(listenerEvent => listenerEvent.RemainingRetryAttempts > 2),
                    TotalEvents = group.LongCount()
                })
                .OrderBy(detail => detail.EventAddressV2Id)
                .ToList();

            long archivedDeadEvents = byAddress.Sum(detail => detail.DeadEvents);

            return new RetryHealthSummaryV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalActiveEvents = byAddress.Sum(detail => detail.TotalEvents) - archivedDeadEvents,
                DeadEvents = archivedDeadEvents,
                CriticalEvents = byAddress.Sum(detail => detail.CriticalEvents),
                HealthyEvents = byAddress.Sum(detail => detail.HealthyEvents),
                ArchivedDeadEvents = archivedDeadEvents,
                Distribution = distribution,
                ByAddress = byAddress
            };
        }
    }
}
