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

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveHealthSummaryV2WithGreenStatusesWhenAllDataIsHealthyAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 3,
                scheduledCount: 2,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 18,
                pendingCount: 1,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 2);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 3,
                errorCount: 1);

            int expectedTotalAddresses = randomEventAddressV2s.Count();
            int expectedTotalListeners = randomEventListenerV2s.Count();
            int expectedTotalEvents = randomEventV2s.Count();
            int expectedImmediateEvents = 3;
            int expectedScheduledEvents = 2;
            int expectedDeadEvents = 0;
            int expectedTotalListenerEvents = randomListenerEventV2s.Count();
            int expectedPendingListenerEvents = 1;
            int expectedSuccessListenerEvents = 18;
            int expectedErrorListenerEvents = 0;
            int expectedTotalArchivedEvents = randomEventArchiveV2s.Count();
            int expectedTotalArchivedListenerEvents = randomListenerEventArchiveV2s.Count();
            int expectedArchivedListenerErrors = 1;
            int expectedHandlerCount = 2;

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Should().HaveCount(15);

            actualResult.Single(i => i.Grouping == "Event Addresses" && i.Item == "Total")
                .Value.Should().Be(expectedTotalAddresses.ToString());

            actualResult.Single(i => i.Grouping == "Event Listeners" && i.Item == "Total")
                .Value.Should().Be(expectedTotalListeners.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Total")
                .Value.Should().Be(expectedTotalEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Immediate")
                .Value.Should().Be(expectedImmediateEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Scheduled")
                .Value.Should().Be(expectedScheduledEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Dead (0 retries)")
                .Value.Should().Be(expectedDeadEvents.ToString());

            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Dead (0 retries)")
                .StatusCode.Should().Be((int)HealthStatusV2.Green);

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

            actualResult.Single(i => i.Grouping == "Event Handlers" && i.Item == "Registered Handlers")
                .Value.Should().Be(expectedHandlerCount.ToString());

            actualResult.Single(i => i.Grouping == "Event Handlers" && i.Item == "Registered Handlers")
                .StatusCode.Should().Be((int)HealthStatusV2.Green);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnAmberForDeadEventsWhenBetweenOneAndFiveAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 3,
                scheduledCount: 2,
                deadCount: 3);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 18,
                pendingCount: 1,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 2);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 3,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Dead (0 retries)")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnRedForDeadEventsWhenMoreThanFiveAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 6);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 18,
                pendingCount: 1,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Dead (0 retries)")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnAmberForListenerEventErrorRateWhenBetweenTenAndTwentyFivePercentAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 8,
                pendingCount: 0,
                errorCount: 2);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnRedForListenerEventErrorRateWhenAboveTwentyFivePercentAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 7,
                pendingCount: 0,
                errorCount: 3);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnRedForHandlersWhenNoneAreRegisteredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 5,
                pendingCount: 1,
                errorCount: 0);

            var emptyHandlers = CreateRandomEventHandlers(count: 0);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Handlers" && i.Item == "Registered Handlers")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNAForDeadEventsWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutDeadEvents = new HealthConfiguration();
            configWithoutDeadEvents.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.DeadEvents);

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 3);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 9,
                pendingCount: 0,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutDeadEvents);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Active Events" && i.Item == "Dead (0 retries)")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNAForErrorRateWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutErrorRate = new HealthConfiguration();

            configWithoutErrorRate.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.ErrorRate);

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 7,
                pendingCount: 0,
                errorCount: 3);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutErrorRate);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNAForHandlerCountWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutHandlerCount = new HealthConfiguration();

            configWithoutHandlerCount.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.HandlerCount);

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 9,
                pendingCount: 0,
                errorCount: 0);

            var emptyHandlers = CreateRandomEventHandlers(count: 0);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutHandlerCount);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Handlers" && i.Item == "Registered Handlers")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
