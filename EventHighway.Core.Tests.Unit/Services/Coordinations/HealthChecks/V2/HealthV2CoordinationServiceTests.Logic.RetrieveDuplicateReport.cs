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

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveDuplicateReportV2Async()
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

            Guid addressIdX = GetRandomId();
            Guid addressIdY = GetRandomId();
            Guid participantIdP = GetRandomId();

            EventAddressUsageV2 nameRowX = CreateNameAddressUsage(addressIdX, GetRandomString());
            ParticipantUsageV2 nameRowP = CreateNameParticipantUsage(participantIdP, GetRandomString());

            var duplicateRowX = new DuplicateDetailV2
            {
                EventAddressV2Id = addressIdX,
                EventParticipantV2Id = participantIdP,
                TotalEvents = GetRandomNumber(),
                Duplicates = GetRandomNumber(),
                DuplicateRate = 12.34m,
                LastDuplicateSeen = GetRandomDateTimeOffset()
            };

            var duplicateRowY = new DuplicateDetailV2
            {
                EventAddressV2Id = addressIdY,
                EventParticipantV2Id = null,
                TotalEvents = GetRandomNumber(),
                Duplicates = GetRandomNumber(),
                DuplicateRate = 5.67m,
                LastDuplicateSeen = GetRandomDateTimeOffset()
            };

            var liveDuplicates = new DuplicateDetectionSummaryV2
            {
                TotalDuplicatesDetected = GetRandomNumber(),
                TotalUniqueEvents = GetRandomNumber(),
                OverallDuplicateRate = 8.90m,
                ByAddress = new List<DuplicateDetailV2> { duplicateRowX, duplicateRowY }
            };

            var infrastructurePartialReport = new HealthReportV2
            {
                AddressUsage = new List<EventAddressUsageV2> { nameRowX },
                ParticipantUsage = new List<ParticipantUsageV2> { nameRowP }
            };

            var eventsPartialReport = new HealthReportV2 { Duplicates = liveDuplicates };

            var expectedDuplicates = new DuplicateDetectionSummaryV2
            {
                Period = inputPeriod,
                WindowStart = inputWindowStart,
                WindowEnd = expectedWindowEnd,
                WindowLabel = expectedWindowLabel,
                TotalDuplicatesDetected = liveDuplicates.TotalDuplicatesDetected,
                TotalUniqueEvents = liveDuplicates.TotalUniqueEvents,
                OverallDuplicateRate = liveDuplicates.OverallDuplicateRate,
                ByAddress = new List<DuplicateDetailV2>
                {
                    new DuplicateDetailV2
                    {
                        EventAddressV2Id = addressIdX,
                        EventAddressV2Name = nameRowX.Name,
                        EventParticipantV2Id = participantIdP,
                        EventParticipantV2Name = nameRowP.Name,
                        TotalEvents = duplicateRowX.TotalEvents,
                        Duplicates = duplicateRowX.Duplicates,
                        DuplicateRate = duplicateRowX.DuplicateRate,
                        LastDuplicateSeen = duplicateRowX.LastDuplicateSeen
                    },

                    new DuplicateDetailV2
                    {
                        EventAddressV2Id = addressIdY,
                        EventAddressV2Name = null,
                        EventParticipantV2Id = null,
                        EventParticipantV2Name = "Unknown",
                        TotalEvents = duplicateRowY.TotalEvents,
                        Duplicates = duplicateRowY.Duplicates,
                        DuplicateRate = duplicateRowY.DuplicateRate,
                        LastDuplicateSeen = duplicateRowY.LastDuplicateSeen
                    }
                }
            };

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

            // when
            HealthReportV2 actualHealthReport =
                await this.healthV2CoordinationService.RetrieveDuplicateReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(expectedWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.Duplicates.Should().BeEquivalentTo(expectedDuplicates);
            actualHealthReport.HealthCheckItems.Should().BeNull();
            actualHealthReport.Traffic.Should().BeNull();
            actualHealthReport.AddressUsage.Should().BeNull();
            actualHealthReport.ParticipantUsage.Should().BeNull();
            actualHealthReport.LoopDetection.Should().BeNull();
            actualHealthReport.Retry.Should().BeNull();

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
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
    }
}
