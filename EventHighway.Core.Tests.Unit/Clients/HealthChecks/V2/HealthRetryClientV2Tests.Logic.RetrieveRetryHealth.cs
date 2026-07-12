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
    public partial class HealthRetryClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveRetryHealthV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            RetryHealthSummaryV2 randomSummary =
                CreateRandomRetryHealthSummaryV2();

            var returnedHealthReport = new HealthReportV2
            {
                Retry = randomSummary
            };

            RetryHealthSummaryV2 expectedSummary =
                randomSummary.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.healthRetryClientV2
                    .RetrieveRetryHealthV2Async(
                        randomPeriod, randomWindowStart, null, randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveRetryHealthV2ForCustomPeriodAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = TrafficPeriodV2.Custom;
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();
            DateTimeOffset randomWindowEnd = randomWindowStart.AddDays(5);

            RetryHealthSummaryV2 randomSummary =
                CreateRandomRetryHealthSummaryV2();

            var returnedHealthReport = new HealthReportV2
            {
                Retry = randomSummary
            };

            RetryHealthSummaryV2 expectedSummary =
                randomSummary.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryReportV2Async(
                    inputPeriod, randomWindowStart, randomWindowEnd, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.healthRetryClientV2
                    .RetrieveRetryHealthV2Async(
                        inputPeriod, randomWindowStart, randomWindowEnd, randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryReportV2Async(
                    inputPeriod, randomWindowStart, randomWindowEnd, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
