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
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RagStatuses.V2
{
    public partial class RagStatusV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveHealthRagStatusV2WithGreenStatusesWhenAllDataIsHealthyAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();

            var randomEventListenerV2s =
                CreateRandomEventListenerV2s(listenerCount: 5, handlerCount: 2);

            var addressesWithListeners =
                AttachEventListenerV2s(randomEventAddressV2s, randomEventListenerV2s);

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 3,
                scheduledCount: 2,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 18,
                pendingCount: 1,
                errorCount: 0,
                replayCount: 0);

            var eventsWithListenerEvents =
                AttachListenerEventV2s(randomEventV2s, randomListenerEventV2s);

            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 18,
                errorCount: 0);

            var archivesWithListenerArchives =
                AttachListenerEventArchiveV2s(randomEventArchiveV2s, randomListenerEventArchiveV2s);

            int expectedTotalAddresses = randomEventAddressV2s.Count();
            int expectedTotalListeners = randomEventListenerV2s.Count();
            int expectedTotalEvents = randomEventV2s.Count();
            int expectedActiveEvents = 5;
            int expectedImmediateEvents = 3;
            int expectedScheduledEvents = 2;
            int expectedTotalListenerEvents = randomListenerEventV2s.Count();
            int expectedPendingListenerEvents = 1;
            int expectedSuccessListenerEvents = 18;
            int expectedErrorListenerEvents = 0;
            int expectedTotalArchivedEvents = randomEventArchiveV2s.Count();
            int expectedTotalArchivedListenerEvents = randomListenerEventArchiveV2s.Count();
            int expectedArchivedListenerErrors = 0;
            int expectedHandlerCount = 2;

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(addressesWithListeners);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(eventsWithListenerEvents);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(archivesWithListenerArchives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Should().HaveCount(20);

            actualResult.Single(i => i.Grouping == "Event Addresses / Event Listeners / Handlers" && i.Item == "Total Addresses")
                .Value.Should().Be(expectedTotalAddresses.ToString());

            actualResult.Single(i => i.Grouping == "Event Addresses / Event Listeners / Handlers" && i.Item == "Total Listeners")
                .Value.Should().Be(expectedTotalListeners.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Total Events")
                .Value.Should().Be(expectedTotalEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Active Events")
                .Value.Should().Be(expectedActiveEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Immediate")
                .Value.Should().Be(expectedImmediateEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Scheduled")
                .Value.Should().Be(expectedScheduledEvents.ToString());

            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Total")
                .Value.Should().Be(expectedTotalListenerEvents.ToString());

            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending")
                .Value.Should().Be(expectedPendingListenerEvents.ToString());

            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Successful")
                .Value.Should().Be(expectedSuccessListenerEvents.ToString());

            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Errors")
                .Value.Should().Be(expectedErrorListenerEvents.ToString());

            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Green);

            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Total Archived Events")
                .Value.Should().Be(expectedTotalArchivedEvents.ToString());

            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Total Archived Listener Events")
                .Value.Should().Be(expectedTotalArchivedListenerEvents.ToString());

            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archived Listener Errors")
                .Value.Should().Be(expectedArchivedListenerErrors.ToString());

            actualResult.Single(i => i.Grouping == "Event Addresses / Event Listeners / Handlers" && i.Item == "Registered Handlers")
                .Value.Should().Be(expectedHandlerCount.ToString());

            actualResult.Single(i => i.Grouping == "Event Addresses / Event Listeners / Handlers" && i.Item == "Registered Handlers")
                .StatusCode.Should().Be((int)HealthStatusV2.Green);

            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                .Value.Should().Be("0");

            actualResult.Single(i => i.Grouping == "Loop Detection" && i.Item == "Quarantined Archives")
                .Value.Should().Be("0");

            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Green);

            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Green);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveActiveEventCountsFilteredByStatusAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 1, scheduledCount: 3, deadCount: 4, quarantinedCount: 4),
                CreateRandomListenerEventV2s(successCount: 1));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 1));

            int expectedTotalEvents = events.Count();

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(5, 2), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Total Events")
                .Value.Should().Be(expectedTotalEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Active Events")
                .Value.Should().Be("8");

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Immediate")
                .Value.Should().Be("5");

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Scheduled")
                .Value.Should().Be("3");

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForListenerEventErrorRateWhenBetweenTenAndTwentyFivePercentAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 8, pendingCount: 0, errorCount: 2));

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
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForListenerEventErrorRateWhenAboveTwentyFivePercentAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 7, pendingCount: 0, errorCount: 3));

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
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForHandlersWhenNoneAreReferencedByListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 5, pendingCount: 1));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 2));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(0, 0), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Addresses / Event Listeners / Handlers" && i.Item == "Registered Handlers")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForErrorRateWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutErrorRate = new HealthConfiguration();
            configWithoutErrorRate.Thresholds.RemoveAll(t => t.Metric == HealthMetric.ErrorRate);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutErrorRate);

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 7, pendingCount: 0, errorCount: 3));

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
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForHandlerCountWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutHandlerCount = new HealthConfiguration();
            configWithoutHandlerCount.Thresholds.RemoveAll(t => t.Metric == HealthMetric.HandlerCount);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutHandlerCount);

            var events = AttachListenerEventV2s(
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomListenerEventV2s(successCount: 9));

            var archives = AttachListenerEventArchiveV2s(
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 2));

            SetupRagStatusFoundationMocks(
                randomCancellationToken, CreateAddressesWithListeners(0, 0), events, archives);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Addresses / Event Listeners / Handlers" && i.Item == "Registered Handlers")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyRagStatusFoundationMocksOnce(randomCancellationToken);
        }
    }
}
