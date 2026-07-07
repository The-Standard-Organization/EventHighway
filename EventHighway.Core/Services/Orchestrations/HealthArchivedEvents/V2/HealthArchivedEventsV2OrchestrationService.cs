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

        public async ValueTask<HealthReportV2> RetrieveHealthReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventArchiveV2> archivedEvents =
                await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync(cancellationToken);

            IQueryable<ListenerEventArchiveV2> archivedListenerEvents =
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);

            DateTimeOffset windowEnd = ComputeWindowEnd(period, windowStart);

            IQueryable<EventArchiveV2> windowEvents = archivedEvents
                .Where(archivedEvent => archivedEvent.ArchivedDate >= windowStart
                    && archivedEvent.ArchivedDate < windowEnd);

            IQueryable<ListenerEventArchiveV2> windowListenerEvents = archivedListenerEvents
                .Where(listenerEvent => listenerEvent.ArchivedDate >= windowStart
                    && listenerEvent.ArchivedDate < windowEnd);

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
                        item: "Total Error",
                        count: totalError,
                        total: totalListenerEvents,
                        description: "Archived listener events that ended in an error state."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Active (Retries Left)",
                        value: totalActiveRetries,
                        description: "Errored archived listener events with retry attempts remaining."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Dead (No Retries)",
                        value: totalDead,
                        description: "Errored archived listener events with no retry attempts remaining.")
                },

                Traffic = MapToTrafficSnapshot(
                    period, windowStart, windowEnd, windowEvents, windowListenerEvents),

                AddressUsage = MapToAddressUsage(windowEvents, windowListenerEvents)
            };
        }

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
            var eventBuckets = windowEvents
                .GroupBy(archivedEvent => MapToBucketStart(period, archivedEvent.ArchivedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    Events = group.LongCount(),
                    ImmediateEvents = group.LongCount(
                        archivedEvent => archivedEvent.Type == EventArchiveTypeV2.Immediate),
                    ScheduledEvents = group.LongCount(
                        archivedEvent => archivedEvent.Type == EventArchiveTypeV2.Scheduled)
                })
                .ToList();

            var listenerBuckets = windowListenerEvents
                .GroupBy(listenerEvent => MapToBucketStart(period, listenerEvent.ArchivedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
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
                    TotalArchivedEvents = group.LongCount()
                })
                .ToList();

            var listenerCounts = windowListenerEvents
                .GroupBy(listenerEvent => listenerEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalArchivedListenerEvents = group.LongCount()
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
                        TotalArchivedListenerEvents = listenerCount?.TotalArchivedListenerEvents ?? 0
                    };
                })
                .ToList();
        }
    }
}
