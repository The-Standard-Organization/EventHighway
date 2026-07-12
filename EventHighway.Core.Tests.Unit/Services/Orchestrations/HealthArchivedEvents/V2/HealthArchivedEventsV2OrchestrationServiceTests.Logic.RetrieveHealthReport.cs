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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthArchivedEvents.V2
{
    public partial class HealthArchivedEventsV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveArchivedEventsAndListenersHealthCheckItemsOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            List<EventArchiveV2> randomArchivedEvents =
                CreateRandomEventArchiveV2s(count: GetRandomNumber());

            List<ListenerEventArchiveV2> randomArchivedListenerEvents =
                CreateRandomListenerEventArchiveV2s(count: GetRandomNumber());

            long totalEvents = randomArchivedEvents.Count;

            long totalQuarantined = randomArchivedEvents
                .Count(archivedEvent => archivedEvent.Status == EventArchiveStatusV2.Quarantined);

            long totalListenerEvents = randomArchivedListenerEvents.Count;

            long totalSuccess = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Success);

            long totalError = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error);

            long totalActiveRetries = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            var expectedHealthCheckItems = new List<HealthCheckItemV2>
            {
                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Total Events",
                    Value = totalEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of archived events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Total Quarantined",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of quarantined archived events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Loops Detected",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of archived events quarantined by loop detection.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Duplicates Blocked",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of archived events blocked as duplicates.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Total Listener Events",
                    Value = totalListenerEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of archived listener events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Total Success",
                    Value = FormatRateValue(totalSuccess, totalListenerEvents),
                    Description = "Archived listener events that completed successfully.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Total Errors",
                    Value = FormatRateValue(totalError, totalListenerEvents),
                    Description = "Archived listener events that ended in an error state.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Active (Items With Retries Left)",
                    Value = totalActiveRetries.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored archived listener events with retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Dead (Items With No Retries)",
                    Value = totalDead.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored archived listener events with no retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                }
            };

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedEvents.AsQueryable());

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.HealthCheckItems.Should().BeEquivalentTo(expectedHealthCheckItems);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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
        public async Task ShouldRetrieveArchivedTrafficOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            long windowSpanTicks = (windowEnd - inputWindowStart).Ticks;

            int inWindowEventCount = GetRandomNumber();

            List<EventArchiveV2> inWindowEvents = Enumerable.Range(start: 0, count: inWindowEventCount)
                .Select(index => CreateRandomEventArchiveV2WithArchivedDate(
                    inputWindowStart.AddTicks(windowSpanTicks * (2 * index + 1) / (2 * inWindowEventCount))))
                .ToList();

            int inWindowListenerEventCount = GetRandomNumber();

            List<ListenerEventArchiveV2> inWindowListenerEvents =
                Enumerable.Range(start: 0, count: inWindowListenerEventCount)
                    .Select(index => CreateRandomListenerEventArchiveV2WithArchivedDate(
                        inputWindowStart.AddTicks(windowSpanTicks * (2 * index + 1) / (2 * inWindowListenerEventCount))))
                    .ToList();

            List<EventArchiveV2> randomArchivedEvents = inWindowEvents
                .Append(CreateRandomEventArchiveV2WithArchivedDate(inputWindowStart.AddTicks(-1)))
                .Append(CreateRandomEventArchiveV2WithArchivedDate(windowEnd))
                .ToList();

            List<ListenerEventArchiveV2> randomArchivedListenerEvents = inWindowListenerEvents
                .Append(CreateRandomListenerEventArchiveV2WithArchivedDate(inputWindowStart.AddTicks(-1)))
                .Append(CreateRandomListenerEventArchiveV2WithArchivedDate(windowEnd))
                .ToList();

            TrafficSnapshotV2 expectedTraffic = BuildExpectedTrafficSnapshot(
                inputPeriod, inputWindowStart, windowEnd, inWindowEvents, inWindowListenerEvents);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedEvents.AsQueryable());

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Traffic.Should().BeEquivalentTo(expectedTraffic);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static TrafficSnapshotV2 BuildExpectedTrafficSnapshot(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            List<EventArchiveV2> windowEvents,
            List<ListenerEventArchiveV2> windowListenerEvents)
        {
            var eventBuckets = windowEvents
                .GroupBy(archivedEvent => GetExpectedBucketStart(period, archivedEvent.ArchivedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    Events = (long)group.Count(),
                    ImmediateEvents = (long)group.Count(
                        archivedEvent => archivedEvent.Type == EventArchiveTypeV2.Immediate),
                    ScheduledEvents = (long)group.Count(
                        archivedEvent => archivedEvent.Type == EventArchiveTypeV2.Scheduled)
                })
                .ToList();

            var listenerBuckets = windowListenerEvents
                .GroupBy(listenerEvent => GetExpectedBucketStart(period, listenerEvent.ArchivedDate))
                .Select(group => new
                {
                    PeriodStart = group.Key,
                    ListenerEvents = (long)group.Count(),
                    Success = (long)group.Count(le => le.Status == ListenerEventArchiveStatusV2.Success),
                    Errors = (long)group.Count(le => le.Status == ListenerEventArchiveStatusV2.Error),
                    Pending = (long)group.Count(le => le.Status == ListenerEventArchiveStatusV2.Pending),
                    Replays = (long)group.Count(le => le.Status == ListenerEventArchiveStatusV2.Replay)
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
                TotalSuccess = windowListenerEvents.Count(le => le.Status == ListenerEventArchiveStatusV2.Success),
                TotalErrors = windowListenerEvents.Count(le => le.Status == ListenerEventArchiveStatusV2.Error),
                TotalPending = windowListenerEvents.Count(le => le.Status == ListenerEventArchiveStatusV2.Pending),
                TotalReplays = windowListenerEvents.Count(le => le.Status == ListenerEventArchiveStatusV2.Replay),
                Buckets = buckets
            };
        }

        private static DateTimeOffset GetExpectedBucketStart(TrafficPeriodV2 period, DateTimeOffset archivedDate)
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

        [Fact]
        public async Task ShouldRetrieveArchivedAddressUsageCountsOnRetrieveHealthReportV2Async()
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

            List<EventArchiveV2> inWindowEvents = Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(index => AssignAddress(
                    CreateRandomEventArchiveV2WithArchivedDate(inWindowDate), addressIds[index % addressIds.Count]))
                .ToList();

            List<ListenerEventArchiveV2> inWindowListenerEvents =
                Enumerable.Range(start: 0, count: GetRandomNumber())
                    .Select(index => AssignAddress(
                        CreateRandomListenerEventArchiveV2WithArchivedDate(inWindowDate),
                        addressIds[index % addressIds.Count]))
                    .Concat(addressIds.Select(addressId => CreateErrorListenerEventArchiveV2(
                        addressId, remainingRetryAttempts: GetRandomNumber(), inWindowDate)))
                    .ToList();

            List<EventArchiveV2> randomArchivedEvents = inWindowEvents
                .Append(AssignAddress(
                    CreateRandomEventArchiveV2WithArchivedDate(outOfWindowDate), addressIds[0]))
                .ToList();

            List<ListenerEventArchiveV2> randomArchivedListenerEvents = inWindowListenerEvents
                .Append(AssignAddress(
                    CreateRandomListenerEventArchiveV2WithArchivedDate(outOfWindowDate), addressIds[0]))
                .ToList();

            IReadOnlyList<EventAddressUsageV2> expectedAddressUsage =
                BuildExpectedAddressUsage(inWindowEvents, inWindowListenerEvents);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedEvents.AsQueryable());

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.AddressUsage.Should().BeEquivalentTo(expectedAddressUsage);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static IReadOnlyList<EventAddressUsageV2> BuildExpectedAddressUsage(
            List<EventArchiveV2> windowEvents,
            List<ListenerEventArchiveV2> windowListenerEvents)
        {
            var eventCounts = windowEvents
                .GroupBy(archivedEvent => archivedEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalArchivedEvents = (long)group.Count(),
                    LastActivity = group.Max(archivedEvent => archivedEvent.CreatedDate)
                })
                .ToList();

            var listenerCounts = windowListenerEvents
                .GroupBy(listenerEvent => listenerEvent.EventAddressV2Id)
                .Select(group => new
                {
                    EventAddressV2Id = group.Key,
                    TotalArchivedListenerEvents = (long)group.Count(),

                    ErrorListenerEvents = (long)group.Count(listenerEvent =>
                        listenerEvent.Status == ListenerEventArchiveStatusV2.Error),

                    LastActivity = group.Max(listenerEvent => listenerEvent.CreatedDate)
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

        private static EventArchiveV2 AssignAddress(EventArchiveV2 eventArchiveV2, Guid eventAddressV2Id)
        {
            eventArchiveV2.EventAddressV2Id = eventAddressV2Id;

            return eventArchiveV2;
        }

        private static ListenerEventArchiveV2 AssignAddress(
            ListenerEventArchiveV2 listenerEventArchiveV2, Guid eventAddressV2Id)
        {
            listenerEventArchiveV2.EventAddressV2Id = eventAddressV2Id;

            return listenerEventArchiveV2;
        }

        [Fact]
        public async Task ShouldRetrieveArchivedLoopDetectionOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            DateTimeOffset earlyDate = inputWindowStart;
            DateTimeOffset laterDate = inputWindowStart.AddTicks((windowEnd - inputWindowStart).Ticks / 2);
            DateTimeOffset outOfWindowDate = inputWindowStart.AddTicks(-1);

            Guid addressA = GetRandomId();
            Guid addressB = GetRandomId();
            Guid participant = GetRandomId();

            List<EventArchiveV2> randomArchivedEvents = new List<EventArchiveV2>
            {
                CreateQuarantinedEventArchiveV2(participant, addressA, earlyDate),
                CreateQuarantinedEventArchiveV2(participant, addressA, laterDate),
                CreateQuarantinedEventArchiveV2(participant, addressB, earlyDate),
                CreateQuarantinedEventArchiveV2(eventParticipantV2Id: null, addressA, laterDate),
                AssignAddress(
                    WithStatus(CreateRandomEventArchiveV2WithArchivedDate(earlyDate), EventArchiveStatusV2.Active),
                    addressA),
                CreateQuarantinedEventArchiveV2(participant, addressA, outOfWindowDate)
            };

            LoopDetectionSummaryV2 expectedLoopDetection = BuildExpectedLoopDetection(
                inputPeriod, inputWindowStart, windowEnd, randomArchivedEvents);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedEvents.AsQueryable());

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventArchiveV2>().AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.LoopDetection.Should().BeEquivalentTo(expectedLoopDetection);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static LoopDetectionSummaryV2 BuildExpectedLoopDetection(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            List<EventArchiveV2> allEvents)
        {
            List<EventArchiveV2> quarantinedEvents = allEvents
                .Where(archivedEvent => archivedEvent.Status == EventArchiveStatusV2.Quarantined
                    && archivedEvent.ArchivedDate >= windowStart
                    && archivedEvent.ArchivedDate < windowEnd)
                .ToList();

            List<LoopDetailV2> byAddress = quarantinedEvents
                .GroupBy(archivedEvent => new { archivedEvent.EventAddressV2Id, archivedEvent.EventParticipantV2Id })
                .Select(group => new LoopDetailV2
                {
                    EventAddressV2Id = group.Key.EventAddressV2Id,
                    EventParticipantV2Id = group.Key.EventParticipantV2Id,
                    ArchivedQuarantined = group.Count(),
                    InWindow = group.Count(),
                    MostRecentDetection = group.Max(archivedEvent => archivedEvent.ArchivedDate)
                })
                .ToList();

            return new LoopDetectionSummaryV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalArchivedQuarantined = quarantinedEvents.Count,
                TotalInWindow = quarantinedEvents.Count,
                ByAddress = byAddress
            };
        }

        private static EventArchiveV2 CreateQuarantinedEventArchiveV2(
            Guid? eventParticipantV2Id, Guid eventAddressV2Id, DateTimeOffset archivedDate)
        {
            EventArchiveV2 eventArchiveV2 = CreateRandomEventArchiveV2WithArchivedDate(archivedDate);
            eventArchiveV2.EventParticipantV2Id = eventParticipantV2Id;
            eventArchiveV2.EventAddressV2Id = eventAddressV2Id;
            eventArchiveV2.Status = EventArchiveStatusV2.Quarantined;

            return eventArchiveV2;
        }

        private static EventArchiveV2 WithStatus(EventArchiveV2 eventArchiveV2, EventArchiveStatusV2 status)
        {
            eventArchiveV2.Status = status;

            return eventArchiveV2;
        }

        [Fact]
        public async Task ShouldRetrieveArchivedRetryOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            DateTimeOffset inWindowDate = inputWindowStart;
            DateTimeOffset outOfWindowDate = inputWindowStart.AddTicks(-1);

            Guid addressA = GetRandomId();

            ListenerEventArchiveV2 successListenerEvent =
                CreateRandomListenerEventArchiveV2WithArchivedDate(inWindowDate);
            successListenerEvent.Status = ListenerEventArchiveStatusV2.Success;

            List<ListenerEventArchiveV2> randomArchivedListenerEvents = new List<ListenerEventArchiveV2>
            {
                CreateErrorListenerEventArchiveV2(addressA, remainingRetryAttempts: 0, inWindowDate),
                CreateErrorListenerEventArchiveV2(addressA, remainingRetryAttempts: 0, inWindowDate),
                CreateErrorListenerEventArchiveV2(addressA, remainingRetryAttempts: 2, inWindowDate),
                CreateErrorListenerEventArchiveV2(addressA, remainingRetryAttempts: 0, outOfWindowDate),
                successListenerEvent
            };

            RetryHealthSummaryV2 expectedRetry = BuildExpectedRetry(
                inputPeriod, inputWindowStart, windowEnd, randomArchivedListenerEvents);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventArchiveV2>().AsQueryable());

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Retry.Should().BeEquivalentTo(expectedRetry);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static RetryHealthSummaryV2 BuildExpectedRetry(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            List<ListenerEventArchiveV2> allListenerEvents)
        {
            List<ListenerEventArchiveV2> errorEvents = allListenerEvents
                .Where(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.ArchivedDate >= windowStart
                    && listenerEvent.ArchivedDate < windowEnd)
                .ToList();

            return new RetryHealthSummaryV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                ArchivedDeadEvents = errorEvents.Count(listenerEvent => listenerEvent.RemainingRetryAttempts == 0)
            };
        }

        private static ListenerEventArchiveV2 CreateErrorListenerEventArchiveV2(
            Guid eventAddressV2Id, int remainingRetryAttempts, DateTimeOffset archivedDate)
        {
            ListenerEventArchiveV2 listenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2WithArchivedDate(archivedDate);

            listenerEventArchiveV2.EventAddressV2Id = eventAddressV2Id;
            listenerEventArchiveV2.Status = ListenerEventArchiveStatusV2.Error;
            listenerEventArchiveV2.RemainingRetryAttempts = remainingRetryAttempts;

            return listenerEventArchiveV2;
        }
    }
}
