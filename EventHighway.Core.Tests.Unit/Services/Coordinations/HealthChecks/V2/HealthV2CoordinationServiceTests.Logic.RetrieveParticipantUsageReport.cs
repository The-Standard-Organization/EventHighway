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
        public async Task ShouldRetrieveParticipantUsageReportV2Async()
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

            Guid participantIdA = GetRandomId();
            Guid participantIdB = GetRandomId();
            Guid addressIdX = GetRandomId();
            Guid addressIdY = GetRandomId();

            EventAddressUsageV2 nameRowX = CreateNameAddressUsage(addressIdX, GetRandomString());
            EventAddressUsageV2 nameRowY = CreateNameAddressUsage(addressIdY, GetRandomString());

            ParticipantUsageV2 nameRowA = CreateNameParticipantUsage(participantIdA, GetRandomString());
            DateTimeOffset baseActivityDate = GetRandomDateTimeOffset();

            var liveRowA = new ParticipantUsageV2
            {
                EventParticipantV2Id = participantIdA,
                TotalEventsSubmitted = 10,
                TotalListenerEvents = 20,
                ErrorListenerEvents = 5,
                LoopsDetected = 0,
                DuplicatesDetected = GetRandomNumber(),
                LastActivity = baseActivityDate.AddHours(1),
                ByAddress = new List<ParticipantAddressUsageV2>
                {
                    new ParticipantAddressUsageV2
                    {
                        EventAddressV2Id = addressIdX,
                        Sent = 10,
                        Received = 20
                    }
                }
            };

            var liveRowB = new ParticipantUsageV2
            {
                EventParticipantV2Id = participantIdB,
                TotalEventsSubmitted = 30,
                TotalListenerEvents = 60,
                ErrorListenerEvents = 6,
                LoopsDetected = 3,
                DuplicatesDetected = GetRandomNumber(),
                LastActivity = baseActivityDate,
                ByAddress = new List<ParticipantAddressUsageV2>
                {
                    new ParticipantAddressUsageV2
                    {
                        EventAddressV2Id = addressIdY,
                        Sent = 30,
                        Received = 60
                    }
                }
            };

            var infrastructurePartialReport = new HealthReportV2
            {
                ParticipantUsage = new List<ParticipantUsageV2> { nameRowA },
                AddressUsage = new List<EventAddressUsageV2> { nameRowX, nameRowY }
            };

            var eventsPartialReport = new HealthReportV2
            {
                ParticipantUsage = new List<ParticipantUsageV2> { liveRowA, liveRowB }
            };

            var expectedParticipantUsage = new List<ParticipantUsageV2>
            {
                new ParticipantUsageV2
                {
                    EventParticipantV2Id = participantIdA,
                    Name = nameRowA.Name,
                    ContactEmail = nameRowA.ContactEmail,
                    ContactPhone = nameRowA.ContactPhone,
                    IsActive = nameRowA.IsActive,
                    OwnedListeners = nameRowA.OwnedListeners,
                    TotalEventsSubmitted = 10,
                    TotalListenerEvents = 20,
                    ErrorListenerEvents = 5,
                    LoopsDetected = 0,
                    DuplicatesDetected = liveRowA.DuplicatesDetected,
                    PublisherErrorRate = 0m,
                    ListenerErrorRate = 25m,
                    LastActivity = liveRowA.LastActivity,
                    ByAddress = new List<ParticipantAddressUsageV2>
                    {
                        new ParticipantAddressUsageV2
                        {
                            EventAddressV2Id = addressIdX,
                            EventAddressV2Name = nameRowX.Name,
                            Sent = 10,
                            SentPercentage = 25m,
                            Received = 20,
                            ReceivedPercentage = 25m
                        }
                    },
                    Status = HealthStatusV2.Green
                },

                new ParticipantUsageV2
                {
                    EventParticipantV2Id = participantIdB,
                    Name = "Unknown",
                    TotalEventsSubmitted = 30,
                    TotalListenerEvents = 60,
                    ErrorListenerEvents = 6,
                    LoopsDetected = 3,
                    DuplicatesDetected = liveRowB.DuplicatesDetected,
                    PublisherErrorRate = 10m,
                    ListenerErrorRate = 10m,
                    LastActivity = liveRowB.LastActivity,
                    ByAddress = new List<ParticipantAddressUsageV2>
                    {
                        new ParticipantAddressUsageV2
                        {
                            EventAddressV2Id = addressIdY,
                            EventAddressV2Name = nameRowY.Name,
                            Sent = 30,
                            SentPercentage = 75m,
                            Received = 60,
                            ReceivedPercentage = 75m
                        }
                    },
                    Status = HealthStatusV2.Amber
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

            // when
            HealthReportV2 actualHealthReport =
                await this.healthV2CoordinationService.RetrieveParticipantUsageReportV2Async(
                    inputPeriod, inputWindowStart, null, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(expectedWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.ParticipantUsage.Should().BeEquivalentTo(expectedParticipantUsage);
            actualHealthReport.HealthCheckItems.Should().BeNull();
            actualHealthReport.Traffic.Should().BeNull();
            actualHealthReport.AddressUsage.Should().BeNull();
            actualHealthReport.LoopDetection.Should().BeNull();
            actualHealthReport.Duplicates.Should().BeNull();
            actualHealthReport.Retry.Should().BeNull();

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
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
