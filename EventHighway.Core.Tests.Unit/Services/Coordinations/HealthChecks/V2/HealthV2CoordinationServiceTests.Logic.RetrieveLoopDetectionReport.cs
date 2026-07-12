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
        public async Task ShouldRetrieveLoopDetectionReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);
            DateTimeOffset expectedWindowEnd = GetWindowEnd(inputPeriod, inputWindowStart);

            string expectedWindowLabel =
                BuildExpectedWindowLabel(inputPeriod, inputWindowStart, expectedWindowEnd);

            DateTimeOffset randomGeneratedDate = GetRandomDateTimeOffset();

            // Default thresholds: LoopsDetected G=0 R=6 (lower-healthier).
            var healthConfiguration = new HealthConfiguration();

            Guid addressIdX = GetRandomId();
            Guid addressIdY = GetRandomId();
            Guid participantIdP = GetRandomId();

            EventAddressUsageV2 nameRowX = CreateNameAddressUsage(addressIdX, GetRandomString());
            ParticipantUsageV2 nameRowP = CreateNameParticipantUsage(participantIdP, GetRandomString());

            DateTimeOffset detectionLiveX = GetRandomDateTimeOffset();
            DateTimeOffset detectionArchivedX = detectionLiveX.AddHours(1);
            DateTimeOffset detectionLiveY = GetRandomDateTimeOffset();

            var liveLoopDetection = new LoopDetectionSummaryV2
            {
                TotalActiveQuarantined = 5,
                TotalArchivedQuarantined = 0,
                TotalInWindow = 5,
                ByAddress = new List<LoopDetailV2>
                {
                    new LoopDetailV2
                    {
                        EventAddressV2Id = addressIdX,
                        EventParticipantV2Id = participantIdP,
                        ActiveQuarantined = 2,
                        ArchivedQuarantined = 0,
                        InWindow = 2,
                        MostRecentDetection = detectionLiveX
                    },

                    new LoopDetailV2
                    {
                        EventAddressV2Id = addressIdY,
                        EventParticipantV2Id = null,
                        ActiveQuarantined = 3,
                        ArchivedQuarantined = 0,
                        InWindow = 3,
                        MostRecentDetection = detectionLiveY
                    }
                }
            };

            var archivedLoopDetection = new LoopDetectionSummaryV2
            {
                TotalActiveQuarantined = 0,
                TotalArchivedQuarantined = 5,
                TotalInWindow = 5,
                ByAddress = new List<LoopDetailV2>
                {
                    new LoopDetailV2
                    {
                        EventAddressV2Id = addressIdX,
                        EventParticipantV2Id = participantIdP,
                        ActiveQuarantined = 0,
                        ArchivedQuarantined = 5,
                        InWindow = 5,
                        MostRecentDetection = detectionArchivedX
                    }
                }
            };

            var infrastructurePartialReport = new HealthReportV2
            {
                AddressUsage = new List<EventAddressUsageV2> { nameRowX },
                ParticipantUsage = new List<ParticipantUsageV2> { nameRowP }
            };

            var eventsPartialReport = new HealthReportV2 { LoopDetection = liveLoopDetection };
            var archivedPartialReport = new HealthReportV2 { LoopDetection = archivedLoopDetection };

            var expectedLoopDetection = new LoopDetectionSummaryV2
            {
                Period = inputPeriod,
                WindowStart = inputWindowStart,
                WindowEnd = expectedWindowEnd,
                WindowLabel = expectedWindowLabel,
                TotalActiveQuarantined = 5,
                TotalArchivedQuarantined = 5,
                TotalInWindow = 10,
                ByAddress = new List<LoopDetailV2>
                {
                    new LoopDetailV2
                    {
                        EventAddressV2Id = addressIdX,
                        EventAddressV2Name = nameRowX.Name,
                        EventParticipantV2Id = participantIdP,
                        EventParticipantV2Name = nameRowP.Name,
                        ActiveQuarantined = 2,
                        ArchivedQuarantined = 5,
                        InWindow = 7,
                        MostRecentDetection = detectionArchivedX,
                        Status = HealthStatusV2.Red
                    },

                    new LoopDetailV2
                    {
                        EventAddressV2Id = addressIdY,
                        EventAddressV2Name = null,
                        EventParticipantV2Id = null,
                        EventParticipantV2Name = "Unknown",
                        ActiveQuarantined = 3,
                        ArchivedQuarantined = 0,
                        InWindow = 3,
                        MostRecentDetection = detectionLiveY,
                        Status = HealthStatusV2.Amber
                    }
                }
            };

            this.configurationBrokerMock.Setup(broker =>
                broker.GetHealthConfiguration())
                    .Returns(healthConfiguration);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomGeneratedDate);

            var mockSequence = new MockSequence();

            this.healthInfrastructureV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ReturnsAsync(infrastructurePartialReport);

            this.healthEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ReturnsAsync(eventsPartialReport);

            this.healthArchivedEventsV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ReturnsAsync(archivedPartialReport);

            // when
            HealthReportV2 actualHealthReport =
                await this.healthV2CoordinationService.RetrieveLoopDetectionReportV2Async(
                    inputPeriod, inputWindowStart, null, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(expectedWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.LoopDetection.Should().BeEquivalentTo(expectedLoopDetection);
            actualHealthReport.HealthCheckItems.Should().BeNull();
            actualHealthReport.Traffic.Should().BeNull();
            actualHealthReport.AddressUsage.Should().BeNull();
            actualHealthReport.ParticipantUsage.Should().BeNull();
            actualHealthReport.Duplicates.Should().BeNull();
            actualHealthReport.Retry.Should().BeNull();

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.healthArchivedEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
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
