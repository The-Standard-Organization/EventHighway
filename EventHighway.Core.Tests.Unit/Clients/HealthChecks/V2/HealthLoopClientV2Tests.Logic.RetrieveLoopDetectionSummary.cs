// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthLoopClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveLoopDetectionSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            LoopDetectionSummaryV2 randomSummary =
                CreateRandomLoopDetectionSummaryV2();

            var returnedHealthReport = new HealthReportV2
            {
                LoopDetection = randomSummary
            };

            LoopDetectionSummaryV2 expectedSummary =
                randomSummary.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            LoopDetectionSummaryV2 actualSummary =
                await this.healthLoopClientV2
                    .RetrieveLoopDetectionSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
