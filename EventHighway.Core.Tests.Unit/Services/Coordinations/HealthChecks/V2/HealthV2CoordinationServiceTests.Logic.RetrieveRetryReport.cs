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
        public async Task ShouldRetrieveRetryReportV2Async()
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

            Guid addressIdX = GetRandomId();
            Guid addressIdY = GetRandomId();

            EventAddressUsageV2 nameRowX = CreateNameAddressUsage(addressIdX, GetRandomString());

            var liveRowX = new RetryAddressDetailV2
            {
                EventAddressV2Id = addressIdX,
                DeadEvents = GetRandomNumber(),
                CriticalEvents = GetRandomNumber(),
                HealthyEvents = GetRandomNumber(),
                TotalEvents = GetRandomNumber()
            };

            var liveRowY = new RetryAddressDetailV2
            {
                EventAddressV2Id = addressIdY,
                DeadEvents = GetRandomNumber(),
                CriticalEvents = GetRandomNumber(),
                HealthyEvents = GetRandomNumber(),
                TotalEvents = GetRandomNumber()
            };

            var distribution = new List<RetryBucketV2>
            {
                new RetryBucketV2 { RemainingRetries = 0, Count = GetRandomNumber() },
                new RetryBucketV2 { RemainingRetries = 1, Count = GetRandomNumber() }
            };

            var liveRetry = new RetryHealthSummaryV2
            {
                TotalActiveEvents = GetRandomNumber(),
                DeadEvents = GetRandomNumber(),
                CriticalEvents = GetRandomNumber(),
                HealthyEvents = GetRandomNumber(),
                Distribution = distribution,
                ByAddress = new List<RetryAddressDetailV2> { liveRowX, liveRowY }
            };

            var archivedRetry = new RetryHealthSummaryV2
            {
                ArchivedDeadEvents = GetRandomNumber()
            };

            var infrastructurePartialReport = new HealthReportV2
            {
                AddressUsage = new List<EventAddressUsageV2> { nameRowX }
            };

            var eventsPartialReport = new HealthReportV2 { Retry = liveRetry };
            var archivedPartialReport = new HealthReportV2 { Retry = archivedRetry };

            var expectedRetry = new RetryHealthSummaryV2
            {
                Period = inputPeriod,
                WindowStart = inputWindowStart,
                WindowEnd = expectedWindowEnd,
                WindowLabel = expectedWindowLabel,
                TotalActiveEvents = liveRetry.TotalActiveEvents,
                DeadEvents = liveRetry.DeadEvents,
                CriticalEvents = liveRetry.CriticalEvents,
                HealthyEvents = liveRetry.HealthyEvents,
                ArchivedDeadEvents = archivedRetry.ArchivedDeadEvents,
                Distribution = distribution,
                ByAddress = new List<RetryAddressDetailV2>
                {
                    new RetryAddressDetailV2
                    {
                        EventAddressV2Id = addressIdX,
                        EventAddressV2Name = nameRowX.Name,
                        DeadEvents = liveRowX.DeadEvents,
                        CriticalEvents = liveRowX.CriticalEvents,
                        HealthyEvents = liveRowX.HealthyEvents,
                        TotalEvents = liveRowX.TotalEvents
                    },

                    new RetryAddressDetailV2
                    {
                        EventAddressV2Id = addressIdY,
                        EventAddressV2Name = null,
                        DeadEvents = liveRowY.DeadEvents,
                        CriticalEvents = liveRowY.CriticalEvents,
                        HealthyEvents = liveRowY.HealthyEvents,
                        TotalEvents = liveRowY.TotalEvents
                    }
                }
            };

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
                await this.healthV2CoordinationService.RetrieveRetryReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.WindowEnd.Should().Be(expectedWindowEnd);
            actualHealthReport.WindowLabel.Should().Be(expectedWindowLabel);
            actualHealthReport.GeneratedDate.Should().Be(randomGeneratedDate);
            actualHealthReport.Retry.Should().BeEquivalentTo(expectedRetry);
            actualHealthReport.HealthCheckItems.Should().BeNull();
            actualHealthReport.Traffic.Should().BeNull();
            actualHealthReport.AddressUsage.Should().BeNull();
            actualHealthReport.ParticipantUsage.Should().BeNull();
            actualHealthReport.LoopDetection.Should().BeNull();
            actualHealthReport.Duplicates.Should().BeNull();

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken),
                    Times.Once);

            this.healthEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken),
                    Times.Once);

            this.healthArchivedEventsV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, randomCancellationToken),
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
