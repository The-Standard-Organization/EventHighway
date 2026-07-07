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

        [Fact]
        public async Task ShouldRetrieveParticipantUsageCountsOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            DateTimeOffset inWindowDate = inputWindowStart;
            DateTimeOffset outOfWindowDate = inputWindowStart.AddTicks(-1);

            List<Guid?> participantIds = new List<Guid?> { GetRandomId(), GetRandomId(), null };
            List<Guid> addressIds = new List<Guid> { GetRandomId(), GetRandomId() };

            List<EventV2> inWindowEvents = Enumerable.Range(start: 0, count: GetRandomNumber() + addressIds.Count)
                .Select(index => AssignParticipantAndAddress(
                    CreateRandomEventV2WithCreatedDate(inWindowDate),
                    participantIds[index % participantIds.Count],
                    addressIds[index % addressIds.Count]))
                .ToList();

            List<ListenerEventV2> inWindowListenerEvents =
                Enumerable.Range(start: 0, count: GetRandomNumber() + addressIds.Count)
                    .Select(index => AssignParticipantAndAddress(
                        CreateRandomListenerEventV2WithCreatedDate(inWindowDate),
                        participantIds[index % participantIds.Count],
                        addressIds[index % addressIds.Count]))
                    .ToList();

            List<EventV2> randomEvents = inWindowEvents
                .Append(AssignParticipantAndAddress(
                    CreateRandomEventV2WithCreatedDate(outOfWindowDate), participantIds[0], addressIds[0]))
                .ToList();

            List<ListenerEventV2> randomListenerEvents = inWindowListenerEvents
                .Append(AssignParticipantAndAddress(
                    CreateRandomListenerEventV2WithCreatedDate(outOfWindowDate), participantIds[0], addressIds[0]))
                .ToList();

            IReadOnlyList<ParticipantUsageV2> expectedParticipantUsage =
                BuildExpectedParticipantUsage(inWindowEvents, inWindowListenerEvents);

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
            actualHealthReport.ParticipantUsage.Should().BeEquivalentTo(expectedParticipantUsage);

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

        private static IReadOnlyList<ParticipantUsageV2> BuildExpectedParticipantUsage(
            List<EventV2> windowEvents,
            List<ListenerEventV2> windowListenerEvents)
        {
            var eventCounts = windowEvents
                .GroupBy(@event => @event.EventParticipantV2Id ?? Guid.Empty)
                .Select(group => new
                {
                    EventParticipantV2Id = group.Key,
                    TotalEventsSubmitted = (long)group.Count(),
                    LoopsDetected = (long)group.Count(@event => @event.Status == EventStatusV2.Quarantined)
                })
                .ToList();

            var listenerCounts = windowListenerEvents
                .GroupBy(listenerEvent => listenerEvent.EventParticipantV2Id ?? Guid.Empty)
                .Select(group => new
                {
                    EventParticipantV2Id = group.Key,
                    TotalListenerEvents = (long)group.Count()
                })
                .ToList();

            var sentCounts = windowEvents
                .GroupBy(@event => new
                {
                    EventParticipantV2Id = @event.EventParticipantV2Id ?? Guid.Empty,
                    @event.EventAddressV2Id
                })
                .Select(group => new
                {
                    group.Key.EventParticipantV2Id,
                    group.Key.EventAddressV2Id,
                    Sent = (long)group.Count()
                })
                .ToList();

            var receivedCounts = windowListenerEvents
                .GroupBy(listenerEvent => new
                {
                    EventParticipantV2Id = listenerEvent.EventParticipantV2Id ?? Guid.Empty,
                    listenerEvent.EventAddressV2Id
                })
                .Select(group => new
                {
                    group.Key.EventParticipantV2Id,
                    group.Key.EventAddressV2Id,
                    Received = (long)group.Count()
                })
                .ToList();

            return eventCounts.Select(count => count.EventParticipantV2Id)
                .Union(listenerCounts.Select(count => count.EventParticipantV2Id))
                .Select(participantId =>
                {
                    var eventCount =
                        eventCounts.FirstOrDefault(count => count.EventParticipantV2Id == participantId);

                    var listenerCount =
                        listenerCounts.FirstOrDefault(count => count.EventParticipantV2Id == participantId);

                    var participantSent = sentCounts
                        .Where(count => count.EventParticipantV2Id == participantId)
                        .ToList();

                    var participantReceived = receivedCounts
                        .Where(count => count.EventParticipantV2Id == participantId)
                        .ToList();

                    List<ParticipantAddressUsageV2> byAddress = participantSent
                        .Select(count => count.EventAddressV2Id)
                        .Union(participantReceived.Select(count => count.EventAddressV2Id))
                        .Select(addressId => new ParticipantAddressUsageV2
                        {
                            EventAddressV2Id = addressId,
                            Sent = participantSent
                                .Where(count => count.EventAddressV2Id == addressId)
                                .Select(count => count.Sent)
                                .FirstOrDefault(),
                            Received = participantReceived
                                .Where(count => count.EventAddressV2Id == addressId)
                                .Select(count => count.Received)
                                .FirstOrDefault()
                        })
                        .ToList();

                    return new ParticipantUsageV2
                    {
                        EventParticipantV2Id = participantId,
                        TotalEventsSubmitted = eventCount?.TotalEventsSubmitted ?? 0,
                        LoopsDetected = eventCount?.LoopsDetected ?? 0,
                        DuplicatesDetected = eventCount?.LoopsDetected ?? 0,
                        TotalListenerEvents = listenerCount?.TotalListenerEvents ?? 0,
                        ByAddress = byAddress
                    };
                })
                .ToList();
        }

        private static EventV2 AssignParticipantAndAddress(
            EventV2 eventV2, Guid? eventParticipantV2Id, Guid eventAddressV2Id)
        {
            eventV2.EventParticipantV2Id = eventParticipantV2Id;
            eventV2.EventAddressV2Id = eventAddressV2Id;

            return eventV2;
        }

        private static ListenerEventV2 AssignParticipantAndAddress(
            ListenerEventV2 listenerEventV2, Guid? eventParticipantV2Id, Guid eventAddressV2Id)
        {
            listenerEventV2.EventParticipantV2Id = eventParticipantV2Id;
            listenerEventV2.EventAddressV2Id = eventAddressV2Id;

            return listenerEventV2;
        }

        [Fact]
        public async Task ShouldRetrieveLoopDetectionOnRetrieveHealthReportV2Async()
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

            List<EventV2> randomEvents = new List<EventV2>
            {
                CreateQuarantinedEventV2(participant, addressA, earlyDate),
                CreateQuarantinedEventV2(participant, addressA, laterDate),
                CreateQuarantinedEventV2(participant, addressB, earlyDate),
                CreateQuarantinedEventV2(eventParticipantV2Id: null, addressA, laterDate),
                AssignParticipantAndAddress(
                    WithStatus(CreateRandomEventV2WithCreatedDate(earlyDate), EventStatusV2.Active),
                    participant, addressA),
                CreateQuarantinedEventV2(participant, addressA, outOfWindowDate)
            };

            LoopDetectionSummaryV2 expectedLoopDetection = BuildExpectedLoopDetection(
                inputPeriod, inputWindowStart, windowEnd, randomEvents);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEvents.AsQueryable());

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventV2>().AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.LoopDetection.Should().BeEquivalentTo(expectedLoopDetection);

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

        private static LoopDetectionSummaryV2 BuildExpectedLoopDetection(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            List<EventV2> allEvents)
        {
            List<EventV2> quarantinedEvents = allEvents
                .Where(@event => @event.Status == EventStatusV2.Quarantined
                    && @event.CreatedDate >= windowStart
                    && @event.CreatedDate < windowEnd)
                .ToList();

            List<LoopDetailV2> byAddress = quarantinedEvents
                .GroupBy(@event => new { @event.EventAddressV2Id, @event.EventParticipantV2Id })
                .Select(group => new LoopDetailV2
                {
                    EventAddressV2Id = group.Key.EventAddressV2Id,
                    EventParticipantV2Id = group.Key.EventParticipantV2Id,
                    ActiveQuarantined = group.Count(),
                    InWindow = group.Count(),
                    MostRecentDetection = group.Max(@event => @event.CreatedDate)
                })
                .ToList();

            return new LoopDetectionSummaryV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalActiveQuarantined = quarantinedEvents.Count,
                TotalInWindow = quarantinedEvents.Count,
                ByAddress = byAddress
            };
        }

        private static EventV2 CreateQuarantinedEventV2(
            Guid? eventParticipantV2Id, Guid eventAddressV2Id, DateTimeOffset createdDate)
        {
            EventV2 eventV2 = CreateRandomEventV2WithCreatedDate(createdDate);
            eventV2.EventParticipantV2Id = eventParticipantV2Id;
            eventV2.EventAddressV2Id = eventAddressV2Id;
            eventV2.Status = EventStatusV2.Quarantined;

            return eventV2;
        }

        private static EventV2 WithStatus(EventV2 eventV2, EventStatusV2 status)
        {
            eventV2.Status = status;

            return eventV2;
        }

        [Fact]
        public async Task ShouldRetrieveDuplicatesOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            long windowSpanTicks = (windowEnd - inputWindowStart).Ticks;
            DateTimeOffset firstDate = inputWindowStart;
            DateTimeOffset secondDate = inputWindowStart.AddTicks(windowSpanTicks / 4);
            DateTimeOffset thirdDate = inputWindowStart.AddTicks(windowSpanTicks / 2);
            DateTimeOffset outOfWindowDate = inputWindowStart.AddTicks(-1);

            Guid addressA = GetRandomId();
            Guid participantOne = GetRandomId();
            Guid participantTwo = GetRandomId();
            string hashOne = GetRandomString();
            string hashTwo = GetRandomString();
            string hashThree = GetRandomString();

            List<EventV2> randomEvents = new List<EventV2>
            {
                CreateEventV2WithHash(participantOne, addressA, hashOne, firstDate),
                CreateEventV2WithHash(participantOne, addressA, hashOne, secondDate),
                CreateEventV2WithHash(participantOne, addressA, hashOne, thirdDate),
                CreateEventV2WithHash(participantOne, addressA, hashTwo, firstDate),
                CreateEventV2WithHash(participantOne, addressA, hashThree, firstDate),
                CreateEventV2WithHash(participantOne, addressA, hashThree, secondDate),
                CreateEventV2WithHash(participantTwo, addressA, hashOne, firstDate),
                CreateEventV2WithHash(participantTwo, addressA, hashOne, secondDate),
                CreateEventV2WithHash(participantOne, addressA, hashOne, outOfWindowDate)
            };

            DuplicateDetectionSummaryV2 expectedDuplicates = BuildExpectedDuplicates(
                inputPeriod, inputWindowStart, windowEnd, randomEvents);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEvents.AsQueryable());

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventV2>().AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Duplicates.Should().BeEquivalentTo(expectedDuplicates);

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

        private static DuplicateDetectionSummaryV2 BuildExpectedDuplicates(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            List<EventV2> allEvents)
        {
            List<EventV2> windowEvents = allEvents
                .Where(@event => @event.CreatedDate >= windowStart && @event.CreatedDate < windowEnd)
                .ToList();

            List<DuplicateDetailV2> byAddress = windowEvents
                .GroupBy(@event => new { @event.EventAddressV2Id, @event.EventParticipantV2Id })
                .Select(group =>
                {
                    List<EventV2> groupEvents = group.ToList();

                    var hashGroups = groupEvents
                        .GroupBy(@event => @event.ContentHash)
                        .ToList();

                    long totalEvents = groupEvents.Count;
                    long duplicates = totalEvents - hashGroups.Count;

                    List<EventV2> duplicateEvents = hashGroups
                        .Where(hashGroup => hashGroup.Count() > 1)
                        .SelectMany(hashGroup => hashGroup.OrderBy(@event => @event.CreatedDate).Skip(1))
                        .ToList();

                    return new DuplicateDetailV2
                    {
                        EventAddressV2Id = group.Key.EventAddressV2Id,
                        EventParticipantV2Id = group.Key.EventParticipantV2Id,
                        TotalEvents = totalEvents,
                        Duplicates = duplicates,
                        DuplicateRate = totalEvents > 0 ? (decimal)duplicates / totalEvents * 100 : 0,
                        LastDuplicateSeen = duplicateEvents.Count > 0
                            ? duplicateEvents.Max(@event => @event.CreatedDate)
                            : (DateTimeOffset?)null
                    };
                })
                .ToList();

            long totalDuplicatesDetected = byAddress.Sum(detail => detail.Duplicates);
            long totalEventsInWindow = byAddress.Sum(detail => detail.TotalEvents);

            return new DuplicateDetectionSummaryV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalDuplicatesDetected = totalDuplicatesDetected,
                TotalUniqueEvents = totalEventsInWindow - totalDuplicatesDetected,
                OverallDuplicateRate = totalEventsInWindow > 0
                    ? (decimal)totalDuplicatesDetected / totalEventsInWindow * 100
                    : 0,
                ByAddress = byAddress
            };
        }

        private static EventV2 CreateEventV2WithHash(
            Guid? eventParticipantV2Id, Guid eventAddressV2Id, string contentHash, DateTimeOffset createdDate)
        {
            EventV2 eventV2 = CreateRandomEventV2WithCreatedDate(createdDate);
            eventV2.EventParticipantV2Id = eventParticipantV2Id;
            eventV2.EventAddressV2Id = eventAddressV2Id;
            eventV2.ContentHash = contentHash;

            return eventV2;
        }
    }
}
