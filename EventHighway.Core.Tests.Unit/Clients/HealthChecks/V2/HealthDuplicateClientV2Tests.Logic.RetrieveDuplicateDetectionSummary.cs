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
    public partial class HealthDuplicateClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveDuplicateDetectionSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            DuplicateDetectionSummaryV2 randomSummary =
                CreateRandomDuplicateDetectionSummaryV2();

            var returnedHealthReport = new HealthReportV2
            {
                Duplicates = randomSummary
            };

            DuplicateDetectionSummaryV2 expectedSummary =
                randomSummary.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            DuplicateDetectionSummaryV2 actualSummary =
                await this.healthDuplicateClientV2
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
