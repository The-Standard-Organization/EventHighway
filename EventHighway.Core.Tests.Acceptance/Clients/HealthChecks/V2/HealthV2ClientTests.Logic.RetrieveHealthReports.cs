// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveHealthCheckItemsV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());

            // when
            IReadOnlyList<HealthCheckItemV2> actualHealthCheckItemV2s =
                await this.clientBroker.RetrieveHealthRagStatusV2Async(period, windowStart);

            // then — whole-system RAG items are always present and grouped, computed server-side
            actualHealthCheckItemV2s.Should().NotBeNull();
            actualHealthCheckItemV2s.Should().NotBeEmpty();

            actualHealthCheckItemV2s.Should().OnlyContain(healthCheckItemV2 =>
                string.IsNullOrWhiteSpace(healthCheckItemV2.Grouping) == false);
        }

        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());
            DateTimeOffset currentHourStart = TruncateToMicroseconds(
                GetCurrentHourStart());

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when
            TrafficSnapshotV2 actualTrafficSnapshotV2 =
                await this.clientBroker.RetrieveTrafficSnapshotV2Async(period, windowStart);

            // then — the snapshot is server-side aggregated and the seeded event lands in the
            // correct hourly bucket (guards the date-part bucketing translation)
            actualTrafficSnapshotV2.Should().NotBeNull();
            actualTrafficSnapshotV2.Period.Should().Be(period);
            actualTrafficSnapshotV2.WindowStart.Should().Be(windowStart);
            actualTrafficSnapshotV2.TotalEvents.Should().BeGreaterThanOrEqualTo(1);
            actualTrafficSnapshotV2.Buckets.Should().NotBeNullOrEmpty();

            TrafficBucketV2 currentHourBucket =
                actualTrafficSnapshotV2.Buckets.SingleOrDefault(bucket =>
                    bucket.PeriodStart == currentHourStart);

            currentHourBucket.Should().NotBeNull(
                "the seeded event's created hour must have its own bucket");

            currentHourBucket.Events.Should().BeGreaterThanOrEqualTo(1);

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }

        [Fact]
        public async Task ShouldRetrieveEventAddressSummaryV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when
            IReadOnlyList<EventAddressUsageV2> actualAddressUsages =
                await this.clientBroker.RetrieveEventAddressSummaryV2Async(period, windowStart);

            // then — the seeded address is present with its live event, server-side aggregated
            actualAddressUsages.Should().NotBeNull();

            EventAddressUsageV2 seededAddressUsage =
                actualAddressUsages.SingleOrDefault(addressUsage =>
                    addressUsage.EventAddressV2Id == seededEventV2.EventAddressV2.Id);

            seededAddressUsage.Should().NotBeNull();
            seededAddressUsage.TotalActiveEvents.Should().BeGreaterThanOrEqualTo(1);

            // the coordination enriches every row with the most recent activity and error rate,
            // both computed server-side (guards the GROUP BY ... MAX(CreatedDate) translation)
            seededAddressUsage.LastActivity.Should().NotBeNull();
            seededAddressUsage.ErrorRate.Should().BeGreaterThanOrEqualTo(0);

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }

        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2ForCustomPeriodAsync()
        {
            // given — a three-day custom window ending tomorrow resolves to daily buckets
            TrafficPeriodV2 period = TrafficPeriodV2.Custom;
            DateTimeOffset currentDayStart = TruncateToMicroseconds(GetCurrentDayWindowStart());
            DateTimeOffset windowStart = currentDayStart.AddDays(-2);
            DateTimeOffset windowEnd = currentDayStart.AddDays(1);

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when
            TrafficSnapshotV2 actualTrafficSnapshotV2 =
                await this.clientBroker.RetrieveTrafficSnapshotV2Async(period, windowStart, windowEnd);

            // then — the snapshot honors the explicit window and the seeded event lands in the
            // current day's bucket (guards the span-derived daily bucketing translation)
            actualTrafficSnapshotV2.Should().NotBeNull();
            actualTrafficSnapshotV2.Period.Should().Be(period);
            actualTrafficSnapshotV2.WindowStart.Should().Be(windowStart);
            actualTrafficSnapshotV2.WindowEnd.Should().Be(windowEnd);
            actualTrafficSnapshotV2.TotalEvents.Should().BeGreaterThanOrEqualTo(1);
            actualTrafficSnapshotV2.Buckets.Should().NotBeNullOrEmpty();

            TrafficBucketV2 currentDayBucket =
                actualTrafficSnapshotV2.Buckets.SingleOrDefault(bucket =>
                    bucket.PeriodStart == currentDayStart);

            currentDayBucket.Should().NotBeNull(
                "the seeded event's created day must have its own bucket");

            currentDayBucket.Events.Should().BeGreaterThanOrEqualTo(1);

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }

        [Fact]
        public async Task ShouldRetrieveParticipantSummaryV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when — resolves server-side against the real database (guards translation)
            IReadOnlyList<ParticipantUsageV2> actualParticipantUsages =
                await this.clientBroker.RetrieveParticipantSummaryV2Async(period, windowStart);

            // then
            actualParticipantUsages.Should().NotBeNull();

            actualParticipantUsages.Should().OnlyContain(participantUsage =>
                participantUsage.TotalEventsSubmitted >= 0);

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }

        [Fact]
        public async Task ShouldRetrieveLoopDetectionSummaryV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when — resolves server-side against the real database (guards translation)
            LoopDetectionSummaryV2 actualLoopDetectionSummaryV2 =
                await this.clientBroker.RetrieveLoopDetectionSummaryV2Async(period, windowStart);

            // then — may be null when there is no quarantined activity; when present it is
            // windowed to the request and non-negative
            if (actualLoopDetectionSummaryV2 is not null)
            {
                actualLoopDetectionSummaryV2.WindowStart.Should().Be(windowStart);
                actualLoopDetectionSummaryV2.TotalInWindow.Should().BeGreaterThanOrEqualTo(0);
            }

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }

        [Fact]
        public async Task ShouldRetrieveDuplicateDetectionSummaryV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when — resolves server-side against the real database (guards the distinct-content-hash
            // translation and the null-hash handling)
            DuplicateDetectionSummaryV2 actualDuplicateDetectionSummaryV2 =
                await this.clientBroker.RetrieveDuplicateDetectionSummaryV2Async(period, windowStart);

            // then
            if (actualDuplicateDetectionSummaryV2 is not null)
            {
                actualDuplicateDetectionSummaryV2.WindowStart.Should().Be(windowStart);
                actualDuplicateDetectionSummaryV2.TotalDuplicatesDetected.Should().BeGreaterThanOrEqualTo(0);
            }

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }

        [Fact]
        public async Task ShouldRetrieveRetryHealthV2Async()
        {
            // given
            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = TruncateToMicroseconds(
                GetCurrentDayWindowStart());

            SeededEventV2 seededEventV2 = await SeedFiredEventV2Async();

            // when — resolves server-side against the real database (guards translation)
            RetryHealthSummaryV2 actualRetryHealthSummaryV2 =
                await this.clientBroker.RetrieveRetryHealthV2Async(period, windowStart);

            // then — may be null when there are no errored listener events; when present it is
            // windowed to the request
            if (actualRetryHealthSummaryV2 is not null)
            {
                actualRetryHealthSummaryV2.WindowStart.Should().Be(windowStart);
                actualRetryHealthSummaryV2.DeadEvents.Should().BeGreaterThanOrEqualTo(0);
            }

            // cleanup
            await CleanupSeededEventV2Async(seededEventV2);
        }
    }
}
