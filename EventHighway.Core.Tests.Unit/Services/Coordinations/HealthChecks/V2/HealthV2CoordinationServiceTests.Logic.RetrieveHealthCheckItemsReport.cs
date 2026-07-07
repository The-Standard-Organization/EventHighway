// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldRetrieveHealthCheckItemsReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset expectedWindowEnd = GetWindowEnd(inputPeriod, inputWindowStart);

            string expectedWindowLabel =
                BuildExpectedWindowLabel(inputPeriod, inputWindowStart, expectedWindowEnd);

            DateTimeOffset randomGeneratedDate = GetRandomDateTimeOffset();

            // Default thresholds: HandlerCount G=1 R=0 (higher-healthier); LoopsDetected G=0 R=6;
            // ErrorRate G=9.99 R=25; DeadEvents G=0 R=6; ArchiveErrorRate G=9.99 R=25; DeadArchivedEvents G=0 R=6.
            var healthConfiguration = new HealthConfiguration();

            var infrastructurePartialReport = new HealthReportV2
            {
                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    CreateHealthCheckItem("Infrastructure", "Total Event Addresses", "42"),
                    CreateHealthCheckItem("Infrastructure", "Registered Handlers", "2")
                }
            };

            var eventsPartialReport = new HealthReportV2
            {
                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    CreateHealthCheckItem("Active Events", "Total Quarantined", "3"),
                    CreateHealthCheckItem("Active Events", "Loops Detected", "0"),
                    CreateHealthCheckItem("Active Events", "Duplicates Blocked", "7"),
                    CreateHealthCheckItem("Active Listeners", "Total Error", "25 (25.00%)"),
                    CreateHealthCheckItem("Active Listeners", "Dead (No Retries)", "0")
                }
            };

            var archivedPartialReport = new HealthReportV2
            {
                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    CreateHealthCheckItem("Archived Events", "Total Quarantined", "9"),
                    CreateHealthCheckItem("Archived Listeners", "Total Error", "1 (5.00%)"),
                    CreateHealthCheckItem("Archived Listeners", "Dead (No Retries)", "7")
                }
            };

            var expectedHealthCheckItems = new List<HealthCheckItemV2>
            {
                CreateHealthCheckItem("Infrastructure", "Total Event Addresses", "42"),
                CreateScoredHealthCheckItem("Infrastructure", "Registered Handlers", "2", HealthStatusV2.Green),
                CreateScoredHealthCheckItem("Active Events", "Total Quarantined", "3", HealthStatusV2.Amber),
                CreateScoredHealthCheckItem("Active Events", "Loops Detected", "0", HealthStatusV2.Green),
                CreateScoredHealthCheckItem("Active Events", "Duplicates Blocked", "7", HealthStatusV2.Red),
                CreateScoredHealthCheckItem("Active Listeners", "Total Error", "25 (25.00%)", HealthStatusV2.Red),
                CreateScoredHealthCheckItem("Active Listeners", "Dead (No Retries)", "0", HealthStatusV2.Green),
                CreateHealthCheckItem("Archived Events", "Total Quarantined", "9"),
                CreateScoredHealthCheckItem("Archived Listeners", "Total Error", "1 (5.00%)", HealthStatusV2.Green),

                CreateScoredHealthCheckItem(
                    "Archived Listeners", "Dead (No Retries)", "7", HealthStatusV2.Red)
            };

            this.configurationBrokerMock.Setup(broker =>
                broker.GetHealthConfiguration())
                    .Returns(healthConfiguration);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomGeneratedDate);

            var mockSequence = new MockSequence();

            this.healthInfrastructureV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken))
                    .ReturnsAsync(infrastructurePartialReport);

            this.healthEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken))
                    .ReturnsAsync(eventsPartialReport);

            this.healthArchivedEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken))
                    .ReturnsAsync(archivedPartialReport);

            // when
            HealthReportV2 actualHealthReport =
                await this.healthV2CoordinationService.RetrieveHealthCheckItemsReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(expectedWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.HealthCheckItems.Should().BeEquivalentTo(expectedHealthCheckItems);
            actualHealthReport.Traffic.Should().BeNull();
            actualHealthReport.AddressUsage.Should().BeNull();
            actualHealthReport.ParticipantUsage.Should().BeNull();
            actualHealthReport.LoopDetection.Should().BeNull();
            actualHealthReport.Duplicates.Should().BeNull();
            actualHealthReport.Retry.Should().BeNull();

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken),
                    Times.Once);

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken),
                    Times.Once);

            this.healthArchivedEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
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
