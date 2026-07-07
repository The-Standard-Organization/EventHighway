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
        public async Task ShouldRetrieveAddressUsageReportV2Async()
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

            // Default thresholds: DeadEvents G=0 R=6; LoopsDetected G=0 R=6 (lower-healthier).
            var healthConfiguration = new HealthConfiguration();

            Guid addressIdA = GetRandomId();
            Guid addressIdB = GetRandomId();
            Guid addressIdC = GetRandomId();

            EventAddressUsageV2 nameRowA = CreateNameAddressUsage(addressIdA, GetRandomString());
            EventAddressUsageV2 nameRowB = CreateNameAddressUsage(addressIdB, GetRandomString());

            var liveRowA = new EventAddressUsageV2
            {
                EventAddressV2Id = addressIdA,
                TotalActiveEvents = GetRandomNumber(),
                TotalListenerEvents = GetRandomNumber(),
                DeadEvents = 6,
                LoopsDetected = 0
            };

            var liveRowB = new EventAddressUsageV2
            {
                EventAddressV2Id = addressIdB,
                TotalActiveEvents = GetRandomNumber(),
                TotalListenerEvents = GetRandomNumber(),
                DeadEvents = 0,
                LoopsDetected = 3
            };

            var archivedRowA = new EventAddressUsageV2
            {
                EventAddressV2Id = addressIdA,
                TotalArchivedEvents = GetRandomNumber(),
                TotalArchivedListenerEvents = GetRandomNumber()
            };

            var archivedRowC = new EventAddressUsageV2
            {
                EventAddressV2Id = addressIdC,
                TotalArchivedEvents = GetRandomNumber(),
                TotalArchivedListenerEvents = GetRandomNumber()
            };

            var infrastructurePartialReport = new HealthReportV2
            {
                AddressUsage = new List<EventAddressUsageV2> { nameRowA, nameRowB }
            };

            var eventsPartialReport = new HealthReportV2
            {
                AddressUsage = new List<EventAddressUsageV2> { liveRowA, liveRowB }
            };

            var archivedPartialReport = new HealthReportV2
            {
                AddressUsage = new List<EventAddressUsageV2> { archivedRowA, archivedRowC }
            };

            var expectedAddressUsage = new List<EventAddressUsageV2>
            {
                new EventAddressUsageV2
                {
                    EventAddressV2Id = addressIdA,
                    Name = nameRowA.Name,
                    Description = nameRowA.Description,
                    ActiveListeners = nameRowA.ActiveListeners,
                    TotalActiveEvents = liveRowA.TotalActiveEvents,
                    TotalListenerEvents = liveRowA.TotalListenerEvents,
                    DeadEvents = 6,
                    LoopsDetected = 0,
                    TotalArchivedEvents = archivedRowA.TotalArchivedEvents,
                    TotalArchivedListenerEvents = archivedRowA.TotalArchivedListenerEvents,
                    Status = HealthStatusV2.Red
                },

                new EventAddressUsageV2
                {
                    EventAddressV2Id = addressIdB,
                    Name = nameRowB.Name,
                    Description = nameRowB.Description,
                    ActiveListeners = nameRowB.ActiveListeners,
                    TotalActiveEvents = liveRowB.TotalActiveEvents,
                    TotalListenerEvents = liveRowB.TotalListenerEvents,
                    DeadEvents = 0,
                    LoopsDetected = 3,
                    Status = HealthStatusV2.Amber
                },

                new EventAddressUsageV2
                {
                    EventAddressV2Id = addressIdC,
                    TotalArchivedEvents = archivedRowC.TotalArchivedEvents,
                    TotalArchivedListenerEvents = archivedRowC.TotalArchivedListenerEvents,
                    Status = HealthStatusV2.Green
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
                await this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(expectedWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.AddressUsage.Should().BeEquivalentTo(expectedAddressUsage);
            actualHealthReport.HealthCheckItems.Should().BeNull();
            actualHealthReport.Traffic.Should().BeNull();
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
