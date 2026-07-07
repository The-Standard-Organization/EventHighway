// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthEvents.V2
{
    public partial class HealthEventsV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveActiveEventsAndListenersHealthCheckItemsOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            List<EventV2> randomEvents = CreateRandomEventV2s(count: GetRandomNumber());
            List<ListenerEventV2> randomListenerEvents = CreateRandomListenerEventV2s(count: GetRandomNumber());

            long totalEvents = randomEvents.Count;

            long totalActive = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Active);

            long totalImmediate = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Immediate);

            long totalScheduled = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Scheduled);

            long totalQuarantined = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Quarantined);

            long totalListenerEvents = randomListenerEvents.Count;

            long totalSuccess = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success);

            long totalError = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error);

            long totalActiveRetries = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            var expectedHealthCheckItems = new List<HealthCheckItemV2>
            {
                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Events",
                    Value = totalEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of events created.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Active",
                    Value = totalActive.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of active events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Immediate",
                    Value = totalImmediate.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of active immediate events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Scheduled",
                    Value = totalScheduled.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of active scheduled events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Quarantined",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of quarantined events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Loops Detected",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of events quarantined by loop detection.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Duplicates Blocked",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of events blocked as duplicates.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Total Listener Events",
                    Value = totalListenerEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of listener events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Total Success",
                    Value = FormatRateValue(totalSuccess, totalListenerEvents),
                    Description = "Listener events that completed successfully.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Total Error",
                    Value = FormatRateValue(totalError, totalListenerEvents),
                    Description = "Listener events that ended in an error state.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Active (Retries Left)",
                    Value = totalActiveRetries.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored listener events with retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Dead (No Retries)",
                    Value = totalDead.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored listener events with no retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                }
            };

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEvents.AsQueryable());

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.HealthCheckItems.Should().BeEquivalentTo(expectedHealthCheckItems);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static string FormatRateValue(long count, long total)
        {
            decimal rate = total == 0
                ? 0
                : (decimal)count * 100 / total;

            return $"{count.ToString(CultureInfo.InvariantCulture)} " +
                $"({rate.ToString("0.00", CultureInfo.InvariantCulture)}%)";
        }

        [Fact]
        public async Task ShouldRetrieveTrafficOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            long windowSpanTicks = (windowEnd - inputWindowStart).Ticks;

            int inWindowEventCount = GetRandomNumber();

            List<EventV2> inWindowEvents = Enumerable.Range(start: 0, count: inWindowEventCount)
                .Select(index => CreateRandomEventV2WithCreatedDate(
                    inputWindowStart.AddTicks(windowSpanTicks * (2 * index + 1) / (2 * inWindowEventCount))))
                .ToList();

            int inWindowListenerEventCount = GetRandomNumber();

            List<ListenerEventV2> inWindowListenerEvents = Enumerable.Range(start: 0, count: inWindowListenerEventCount)
                .Select(index => CreateRandomListenerEventV2WithCreatedDate(
                    inputWindowStart.AddTicks(windowSpanTicks * (2 * index + 1) / (2 * inWindowListenerEventCount))))
                .ToList();

            List<EventV2> randomEvents = inWindowEvents
                .Append(CreateRandomEventV2WithCreatedDate(inputWindowStart.AddTicks(-1)))
                .Append(CreateRandomEventV2WithCreatedDate(windowEnd))
                .ToList();

            List<ListenerEventV2> randomListenerEvents = inWindowListenerEvents
                .Append(CreateRandomListenerEventV2WithCreatedDate(inputWindowStart.AddTicks(-1)))
                .Append(CreateRandomListenerEventV2WithCreatedDate(windowEnd))
                .ToList();

            TrafficSnapshotV2 expectedTraffic = BuildExpectedTrafficSnapshot(
                inputPeriod, inputWindowStart, windowEnd, inWindowEvents, inWindowListenerEvents);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEvents.AsQueryable());

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Traffic.Should().BeEquivalentTo(expectedTraffic);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static TrafficSnapshotV2 BuildExpectedTrafficSnapshot(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            List<EventV2> windowEvents,
            List<ListenerEventV2> windowListenerEvents)
        {
            var eventBuckets = windowEvents
                .GroupBy(@event => GetExpectedBucketStart(period, @event.CreatedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    Events = (long)group.Count(),
                    ImmediateEvents = (long)group.Count(@event => @event.Type == EventTypeV2.Immediate),
                    ScheduledEvents = (long)group.Count(@event => @event.Type == EventTypeV2.Scheduled)
                })
                .ToList();

            var listenerBuckets = windowListenerEvents
                .GroupBy(listenerEvent => GetExpectedBucketStart(period, listenerEvent.CreatedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    ListenerEvents = (long)group.Count(),
                    Success = (long)group.Count(le => le.Status == ListenerEventStatusV2.Success),
                    Errors = (long)group.Count(le => le.Status == ListenerEventStatusV2.Error),
                    Pending = (long)group.Count(le => le.Status == ListenerEventStatusV2.Pending),
                    Replays = (long)group.Count(le => le.Status == ListenerEventStatusV2.Replay)
                })
                .ToList();

            List<TrafficBucketV2> buckets = eventBuckets.Select(bucket => bucket.PeriodStart)
                .Union(listenerBuckets.Select(bucket => bucket.PeriodStart))
                .OrderBy(periodStart => periodStart)
                .Select(periodStart =>
                {
                    var eventBucket = eventBuckets.FirstOrDefault(bucket => bucket.PeriodStart == periodStart);
                    var listenerBucket = listenerBuckets.FirstOrDefault(bucket => bucket.PeriodStart == periodStart);

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

            return new TrafficSnapshotV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalEvents = windowEvents.Count,
                TotalListenerEvents = windowListenerEvents.Count,
                TotalSuccess = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Success),
                TotalErrors = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error),
                TotalPending = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Pending),
                TotalReplays = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Replay),
                Buckets = buckets
            };
        }

        private static DateTimeOffset GetExpectedBucketStart(TrafficPeriodV2 period, DateTimeOffset createdDate)
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

        [Fact]
        public async Task ShouldRetrieveAddressUsageCountsOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            DateTimeOffset inWindowDate = inputWindowStart;
            DateTimeOffset outOfWindowDate = inputWindowStart.AddTicks(-1);

            List<Guid> addressIds = Enumerable.Range(start: 0, count: 3)
                .Select(index => GetRandomId())
                .ToList();

            List<EventV2> inWindowEvents = Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(index => AssignAddress(
                    CreateRandomEventV2WithCreatedDate(inWindowDate), addressIds[index % addressIds.Count]))
                .ToList();

            List<ListenerEventV2> inWindowListenerEvents = Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(index => AssignAddress(
                    CreateRandomListenerEventV2WithCreatedDate(inWindowDate), addressIds[index % addressIds.Count]))
                .ToList();

            List<EventV2> randomEvents = inWindowEvents
                .Append(AssignAddress(CreateRandomEventV2WithCreatedDate(outOfWindowDate), addressIds[0]))
                .ToList();

            List<ListenerEventV2> randomListenerEvents = inWindowListenerEvents
                .Append(AssignAddress(CreateRandomListenerEventV2WithCreatedDate(outOfWindowDate), addressIds[0]))
                .ToList();

            IReadOnlyList<EventAddressUsageV2> expectedAddressUsage =
                BuildExpectedAddressUsage(inWindowEvents, inWindowListenerEvents);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEvents.AsQueryable());

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.AddressUsage.Should().BeEquivalentTo(expectedAddressUsage);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static IReadOnlyList<EventAddressUsageV2> BuildExpectedAddressUsage(
            List<EventV2> windowEvents,
            List<ListenerEventV2> windowListenerEvents)
        {
            var eventCounts = windowEvents
                .GroupBy(@event => @event.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalActiveEvents = (long)group.Count(),
                    LoopsDetected = (long)group.Count(@event => @event.Status == EventStatusV2.Quarantined)
                })
                .ToList();

            var listenerCounts = windowListenerEvents
                .GroupBy(listenerEvent => listenerEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalListenerEvents = (long)group.Count(),
                    DeadEvents = (long)group.Count(listenerEvent =>
                        listenerEvent.Status == ListenerEventStatusV2.Error
                        && listenerEvent.RemainingRetryAttempts == 0)
                })
                .ToList();

            return eventCounts.Select(count => count.EventAddressV2Id)
                .Union(listenerCounts.Select(count => count.EventAddressV2Id))
                .Select(addressId =>
                {
                    var eventCount = eventCounts.FirstOrDefault(count => count.EventAddressV2Id == addressId);
                    var listenerCount = listenerCounts.FirstOrDefault(count => count.EventAddressV2Id == addressId);

                    return new EventAddressUsageV2
                    {
                        EventAddressV2Id = addressId,
                        TotalActiveEvents = eventCount?.TotalActiveEvents ?? 0,
                        LoopsDetected = eventCount?.LoopsDetected ?? 0,
                        TotalListenerEvents = listenerCount?.TotalListenerEvents ?? 0,
                        DeadEvents = listenerCount?.DeadEvents ?? 0
                    };
                })
                .ToList();
        }

        private static EventV2 AssignAddress(EventV2 eventV2, Guid eventAddressV2Id)
        {
            eventV2.EventAddressV2Id = eventAddressV2Id;

            return eventV2;
        }

        private static ListenerEventV2 AssignAddress(ListenerEventV2 listenerEventV2, Guid eventAddressV2Id)
        {
            listenerEventV2.EventAddressV2Id = eventAddressV2Id;

            return listenerEventV2;
        }
    }
}
