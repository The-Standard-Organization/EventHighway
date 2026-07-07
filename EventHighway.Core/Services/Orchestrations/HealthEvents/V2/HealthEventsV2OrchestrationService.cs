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
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.HealthEvents.V2
{
    internal partial class HealthEventsV2OrchestrationService : IHealthEventsV2OrchestrationService
    {
        private readonly IEventV2Service eventV2Service;
        private readonly IListenerEventV2Service listenerEventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public HealthEventsV2OrchestrationService(
            IEventV2Service eventV2Service,
            IListenerEventV2Service listenerEventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventV2Service = eventV2Service;
            this.listenerEventV2Service = listenerEventV2Service;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<HealthReportV2> RetrieveHealthReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventV2> events =
                await this.eventV2Service.RetrieveAllEventV2sAsync(cancellationToken);

            IQueryable<ListenerEventV2> listenerEvents =
                await this.listenerEventV2Service.RetrieveAllListenerEventV2sAsync(cancellationToken);

            DateTimeOffset windowEnd = ComputeWindowEnd(period, windowStart);

            IQueryable<EventV2> windowEvents = events
                .Where(@event => @event.CreatedDate >= windowStart
                    && @event.CreatedDate < windowEnd);

            IQueryable<ListenerEventV2> windowListenerEvents = listenerEvents
                .Where(listenerEvent => listenerEvent.CreatedDate >= windowStart
                    && listenerEvent.CreatedDate < windowEnd);

            long totalEvents = events.LongCount();

            long totalActive = events
                .LongCount(@event => @event.Status == EventStatusV2.Active);

            long totalImmediate = events
                .LongCount(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Immediate);

            long totalScheduled = events
                .LongCount(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Scheduled);

            long totalQuarantined = events
                .LongCount(@event => @event.Status == EventStatusV2.Quarantined);

            long totalListenerEvents = listenerEvents.LongCount();

            long totalSuccess = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success);

            long totalError = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error);

            long totalActiveRetries = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,

                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Events",
                        value: totalEvents,
                        description: "Total number of events created."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Active",
                        value: totalActive,
                        description: "Total number of active events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Immediate",
                        value: totalImmediate,
                        description: "Total number of active immediate events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Scheduled",
                        value: totalScheduled,
                        description: "Total number of active scheduled events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Quarantined",
                        value: totalQuarantined,
                        description: "Total number of quarantined events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Loops Detected",
                        value: totalQuarantined,
                        description: "Number of events quarantined by loop detection."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Duplicates Blocked",
                        value: totalQuarantined,
                        description: "Number of events blocked as duplicates."),

                    MapToHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Total Listener Events",
                        value: totalListenerEvents,
                        description: "Total number of listener events."),

                    MapToRateHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Total Success",
                        count: totalSuccess,
                        total: totalListenerEvents,
                        description: "Listener events that completed successfully."),

                    MapToRateHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Total Error",
                        count: totalError,
                        total: totalListenerEvents,
                        description: "Listener events that ended in an error state."),

                    MapToHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Active (Retries Left)",
                        value: totalActiveRetries,
                        description: "Errored listener events with retry attempts remaining."),

                    MapToHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Dead (No Retries)",
                        value: totalDead,
                        description: "Errored listener events with no retry attempts remaining.")
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
            IQueryable<EventV2> windowEvents,
            IQueryable<ListenerEventV2> windowListenerEvents)
        {
            return new TrafficSnapshotV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalEvents = windowEvents.LongCount(),
                TotalListenerEvents = windowListenerEvents.LongCount(),

                TotalSuccess = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success),

                TotalErrors = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error),

                TotalPending = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Pending),

                TotalReplays = windowListenerEvents
                    .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Replay),

                Buckets = MapToTrafficBuckets(period, windowEvents, windowListenerEvents)
            };
        }

        private static IEnumerable<TrafficBucketV2> MapToTrafficBuckets(
            TrafficPeriodV2 period,
            IQueryable<EventV2> windowEvents,
            IQueryable<ListenerEventV2> windowListenerEvents)
        {
            var eventBuckets = windowEvents
                .GroupBy(@event => MapToBucketStart(period, @event.CreatedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    Events = group.LongCount(),
                    ImmediateEvents = group.LongCount(@event => @event.Type == EventTypeV2.Immediate),
                    ScheduledEvents = group.LongCount(@event => @event.Type == EventTypeV2.Scheduled)
                })
                .ToList();

            var listenerBuckets = windowListenerEvents
                .GroupBy(listenerEvent => MapToBucketStart(period, listenerEvent.CreatedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    ListenerEvents = group.LongCount(),
                    Success = group.LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success),
                    Errors = group.LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error),
                    Pending = group.LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Pending),
                    Replays = group.LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Replay)
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

        private static DateTimeOffset MapToBucketStart(TrafficPeriodV2 period, DateTimeOffset createdDate)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(
                        createdDate.Year, createdDate.Month, createdDate.Day, 0, 0, 0, TimeSpan.Zero);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(
                        createdDate.Year, createdDate.Month, 1, 0, 0, 0, TimeSpan.Zero);

                default:
                    return new DateTimeOffset(
                        createdDate.Year, createdDate.Month, createdDate.Day, createdDate.Hour, 0, 0, TimeSpan.Zero);
            }
        }

        private static IReadOnlyList<EventAddressUsageV2> MapToAddressUsage(
            IQueryable<EventV2> windowEvents,
            IQueryable<ListenerEventV2> windowListenerEvents)
        {
            var eventCounts = windowEvents
                .GroupBy(@event => @event.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalActiveEvents = group.LongCount(),
                    LoopsDetected = group.LongCount(@event => @event.Status == EventStatusV2.Quarantined)
                })
                .ToList();

            var listenerCounts = windowListenerEvents
                .GroupBy(listenerEvent => listenerEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalListenerEvents = group.LongCount(),
                    DeadEvents = group.LongCount(listenerEvent =>
                        listenerEvent.Status == ListenerEventStatusV2.Error
                        && listenerEvent.RemainingRetryAttempts == 0)
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
                        TotalActiveEvents = eventCount?.TotalActiveEvents ?? 0,
                        LoopsDetected = eventCount?.LoopsDetected ?? 0,
                        TotalListenerEvents = listenerCount?.TotalListenerEvents ?? 0,
                        DeadEvents = listenerCount?.DeadEvents ?? 0
                    };
                })
                .ToList();
        }
    }
}
