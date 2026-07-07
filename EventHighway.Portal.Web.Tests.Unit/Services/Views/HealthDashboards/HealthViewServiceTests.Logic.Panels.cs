// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.HealthDashboards
{
    public partial class HealthViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotAsync()
        {
            // given
            var snapshot = new TrafficSnapshotV2 { TotalEvents = 42 };

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(snapshot);

            // when
            TrafficSnapshotV2 actual =
                await this.healthViewService.RetrieveTrafficSnapshotAsync(
                    TrafficPeriodV2.Day, DateTimeOffset.MinValue,
                    TestContext.Current.CancellationToken);

            // then
            actual.Should().BeSameAs(snapshot);
        }

        [Fact]
        public async Task ShouldRetrieveAddressSummariesAsync()
        {
            // given
            IReadOnlyList<EventAddressUsageV2> summaries = new List<EventAddressUsageV2>
            {
                new EventAddressUsageV2 { Name = "A" },
                new EventAddressUsageV2 { Name = "B" },
            };

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(summaries);

            // when
            List<EventAddressUsageV2> actual =
                await this.healthViewService.RetrieveAddressSummariesAsync(
                    TrafficPeriodV2.Day, DateTimeOffset.MinValue,
                    TestContext.Current.CancellationToken);

            // then
            actual.Should().BeEquivalentTo(summaries);
        }

        [Fact]
        public async Task ShouldRetrieveLoopSummaryAsync()
        {
            // given
            var summary = new LoopDetectionSummaryV2 { TotalInWindow = 3 };

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(summary);

            // when
            LoopDetectionSummaryV2 actual =
                await this.healthViewService.RetrieveLoopSummaryAsync(
                    TrafficPeriodV2.Day, DateTimeOffset.MinValue,
                    TestContext.Current.CancellationToken);

            // then
            actual.Should().BeSameAs(summary);
        }

        [Fact]
        public async Task ShouldRetrieveDuplicateSummaryAsync()
        {
            // given
            var summary = new DuplicateDetectionSummaryV2 { TotalDuplicatesDetected = 7 };

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(summary);

            // when
            DuplicateDetectionSummaryV2 actual =
                await this.healthViewService.RetrieveDuplicateSummaryAsync(
                    TrafficPeriodV2.Day, DateTimeOffset.MinValue,
                    TestContext.Current.CancellationToken);

            // then
            actual.Should().BeSameAs(summary);
        }

        [Fact]
        public async Task ShouldRetrieveRetryHealthAsync()
        {
            // given
            var summary = new RetryHealthSummaryV2 { DeadEvents = 5 };

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveRetryHealthV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(summary);

            // when
            RetryHealthSummaryV2 actual =
                await this.healthViewService.RetrieveRetryHealthAsync(
                    TrafficPeriodV2.Day, DateTimeOffset.MinValue,
                    TestContext.Current.CancellationToken);

            // then
            actual.Should().BeSameAs(summary);
        }

        [Fact]
        public async Task ShouldRetrieveParticipantSummariesAsync()
        {
            // given
            IReadOnlyList<ParticipantUsageV2> summaries = new List<ParticipantUsageV2>
            {
                new ParticipantUsageV2 { Name = "P1" },
            };

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(summaries);

            // when
            List<ParticipantUsageV2> actual =
                await this.healthViewService.RetrieveParticipantSummariesAsync(
                    TrafficPeriodV2.Day, DateTimeOffset.MinValue,
                    TestContext.Current.CancellationToken);

            // then
            actual.Should().BeEquivalentTo(summaries);
        }
    }
}
