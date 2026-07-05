// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RagStatuses.V2
{
    public partial class RagStatusV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldIncludeLoopDetectionGroupingWithQuarantinedCountsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            const int quarantinedEventCount = 2;
            const int quarantinedArchiveCount = 3;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(
                    immediateCount: 2,
                    scheduledCount: 1,
                    deadCount: 0,
                    quarantinedCount: quarantinedEventCount),
                CreateRandomListenerEventV2s(successCount: 5));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(quarantinedCount: quarantinedArchiveCount),
                CreateRandomListenerEventArchiveV2s(successCount: 2));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                .Value.Should().Be(quarantinedEventCount.ToString());

            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Archives")
                .Value.Should().Be(quarantinedArchiveCount.ToString());

            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Archives")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForLoopsDetectedWhenQuarantinedEventCountIsSixOrMoreAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(
                    immediateCount: 2,
                    scheduledCount: 1,
                    deadCount: 0,
                    quarantinedCount: 6),
                CreateRandomListenerEventV2s(successCount: 5));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 2));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForLoopsDetectedWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutLoopsDetected = new HealthConfiguration();
            configWithoutLoopsDetected.Thresholds.RemoveAll(t => t.Metric == HealthMetric.LoopsDetected);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutLoopsDetected);

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(
                    immediateCount: 2,
                    scheduledCount: 1,
                    deadCount: 0,
                    quarantinedCount: 3),
                CreateRandomListenerEventV2s(successCount: 5));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 2));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForPendingBacklogWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 100, errorCount: 0));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending Listener Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForPendingBacklogWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 500, errorCount: 0));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending Listener Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForPendingBacklogWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutPendingBacklog = new HealthConfiguration();
            configWithoutPendingBacklog.Thresholds.RemoveAll(t => t.Metric == HealthMetric.PendingBacklog);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutPendingBacklog);

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 100, errorCount: 0));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending Listener Events")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForReplayRateWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0, replayCount: 1));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForReplayRateWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 7, pendingCount: 0, errorCount: 0, replayCount: 3));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForReplayRateWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutReplayRate = new HealthConfiguration();
            configWithoutReplayRate.Thresholds.RemoveAll(t => t.Metric == HealthMetric.ReplayRate);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutReplayRate);

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 7, pendingCount: 0, errorCount: 0, replayCount: 3));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForArchiveErrorRateWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 8, errorCount: 2));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForArchiveErrorRateWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 7, errorCount: 3));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForArchiveErrorRateWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutArchiveErrorRate = new HealthConfiguration();
            configWithoutArchiveErrorRate.Thresholds.RemoveAll(t => t.Metric == HealthMetric.ArchiveErrorRate);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutArchiveErrorRate);

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 7, errorCount: 3));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 1), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }
    }
}
