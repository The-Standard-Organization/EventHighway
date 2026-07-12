// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveTrafficReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset windowEnd = GetWindowEnd(inputPeriod, inputWindowStart);
            string expectedWindowLabel = BuildExpectedWindowLabel(inputPeriod, inputWindowStart, windowEnd);
            DateTimeOffset randomGeneratedDate = GetRandomDateTimeOffset();

            List<(DateTimeOffset Start, string Label)> bucketStarts =
                GetExpectedBucketStarts(inputPeriod, inputWindowStart, windowEnd);

            List<TrafficBucketV2> liveBuckets = new List<TrafficBucketV2>
            {
                CreateRandomTrafficBucket(bucketStarts[0].Start),
                CreateRandomTrafficBucket(bucketStarts[1].Start)
            };

            List<TrafficBucketV2> archivedBuckets = new List<TrafficBucketV2>
            {
                CreateRandomTrafficBucket(bucketStarts[1].Start),
                CreateRandomTrafficBucket(bucketStarts[2].Start)
            };

            TrafficSnapshotV2 liveTraffic = CreateRandomTrafficSnapshot(
                inputPeriod, inputWindowStart, windowEnd, liveBuckets);

            TrafficSnapshotV2 archivedTraffic = CreateRandomTrafficSnapshot(
                inputPeriod, inputWindowStart, windowEnd, archivedBuckets);

            var eventsPartialReport = new HealthReportV2 { Traffic = liveTraffic };
            var archivedPartialReport = new HealthReportV2 { Traffic = archivedTraffic };

            List<TrafficBucketV2> expectedBuckets = bucketStarts
                .Select(bucket =>
                {
                    TrafficBucketV2 liveBucket =
                        liveBuckets.FirstOrDefault(candidate => candidate.PeriodStart == bucket.Start);

                    TrafficBucketV2 archivedBucket =
                        archivedBuckets.FirstOrDefault(candidate => candidate.PeriodStart == bucket.Start);

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

            var expectedTraffic = new TrafficSnapshotV2
            {
                Period = inputPeriod,
                WindowStart = inputWindowStart,
                WindowEnd = windowEnd,
                WindowLabel = expectedWindowLabel,
                TotalEvents = liveTraffic.TotalEvents + archivedTraffic.TotalEvents,
                TotalListenerEvents = liveTraffic.TotalListenerEvents + archivedTraffic.TotalListenerEvents,
                TotalSuccess = liveTraffic.TotalSuccess + archivedTraffic.TotalSuccess,
                TotalErrors = liveTraffic.TotalErrors + archivedTraffic.TotalErrors,
                TotalPending = liveTraffic.TotalPending + archivedTraffic.TotalPending,
                TotalReplays = liveTraffic.TotalReplays + archivedTraffic.TotalReplays,
                Buckets = expectedBuckets
            };

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomGeneratedDate);

            var mockSequence = new MockSequence();

            this.healthEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ReturnsAsync(eventsPartialReport);

            this.healthArchivedEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ReturnsAsync(archivedPartialReport);

            // when
            HealthReportV2 actualHealthReport =
                await this.healthV2CoordinationService.RetrieveTrafficReportV2Async(
                    inputPeriod, inputWindowStart, null, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(windowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.Traffic.Should().BeEquivalentTo(expectedTraffic);
            actualHealthReport.HealthCheckItems.Should().BeNull();
            actualHealthReport.AddressUsage.Should().BeNull();
            actualHealthReport.ParticipantUsage.Should().BeNull();
            actualHealthReport.LoopDetection.Should().BeNull();
            actualHealthReport.Duplicates.Should().BeNull();
            actualHealthReport.Retry.Should().BeNull();

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.healthArchivedEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveTrafficReportV2ForCustomPeriodAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = TrafficPeriodV2.Custom;
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(TrafficPeriodV2.Week);
            DateTimeOffset inputWindowEnd = inputWindowStart.AddDays(5);

            string expectedWindowLabel =
                BuildExpectedWindowLabel(inputPeriod, inputWindowStart, inputWindowEnd);

            DateTimeOffset randomGeneratedDate = GetRandomDateTimeOffset();

            List<(DateTimeOffset Start, string Label)> bucketStarts =
                GetExpectedBucketStarts(inputPeriod, inputWindowStart, inputWindowEnd);

            List<TrafficBucketV2> liveBuckets = new List<TrafficBucketV2>
            {
                CreateRandomTrafficBucket(bucketStarts[0].Start),
                CreateRandomTrafficBucket(bucketStarts[1].Start)
            };

            List<TrafficBucketV2> archivedBuckets = new List<TrafficBucketV2>
            {
                CreateRandomTrafficBucket(bucketStarts[1].Start),
                CreateRandomTrafficBucket(bucketStarts[2].Start)
            };

            TrafficSnapshotV2 liveTraffic = CreateRandomTrafficSnapshot(
                inputPeriod, inputWindowStart, inputWindowEnd, liveBuckets);

            TrafficSnapshotV2 archivedTraffic = CreateRandomTrafficSnapshot(
                inputPeriod, inputWindowStart, inputWindowEnd, archivedBuckets);

            var eventsPartialReport = new HealthReportV2 { Traffic = liveTraffic };
            var archivedPartialReport = new HealthReportV2 { Traffic = archivedTraffic };

            List<TrafficBucketV2> expectedBuckets = bucketStarts
                .Select(bucket =>
                {
                    TrafficBucketV2 liveBucket =
                        liveBuckets.FirstOrDefault(candidate => candidate.PeriodStart == bucket.Start);

                    TrafficBucketV2 archivedBucket =
                        archivedBuckets.FirstOrDefault(candidate => candidate.PeriodStart == bucket.Start);

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

            var expectedTraffic = new TrafficSnapshotV2
            {
                Period = inputPeriod,
                WindowStart = inputWindowStart,
                WindowEnd = inputWindowEnd,
                WindowLabel = expectedWindowLabel,
                TotalEvents = liveTraffic.TotalEvents + archivedTraffic.TotalEvents,
                TotalListenerEvents = liveTraffic.TotalListenerEvents + archivedTraffic.TotalListenerEvents,
                TotalSuccess = liveTraffic.TotalSuccess + archivedTraffic.TotalSuccess,
                TotalErrors = liveTraffic.TotalErrors + archivedTraffic.TotalErrors,
                TotalPending = liveTraffic.TotalPending + archivedTraffic.TotalPending,
                TotalReplays = liveTraffic.TotalReplays + archivedTraffic.TotalReplays,
                Buckets = expectedBuckets
            };

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomGeneratedDate);

            var mockSequence = new MockSequence();

            this.healthEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(
                    inputPeriod, inputWindowStart, inputWindowEnd, randomCancellationToken))
                        .ReturnsAsync(eventsPartialReport);

            this.healthArchivedEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(
                    inputPeriod, inputWindowStart, inputWindowEnd, randomCancellationToken))
                        .ReturnsAsync(archivedPartialReport);

            // when
            HealthReportV2 actualHealthReport =
                await this.healthV2CoordinationService.RetrieveTrafficReportV2Async(
                    inputPeriod, inputWindowStart, inputWindowEnd, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(inputWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.Traffic.Should().BeEquivalentTo(expectedTraffic);

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(
                    inputPeriod, inputWindowStart, inputWindowEnd, randomCancellationToken),
                        Times.Once);

            this.healthArchivedEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(
                    inputPeriod, inputWindowStart, inputWindowEnd, randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
