// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthAddressClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveEventAddressSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            IReadOnlyList<EventAddressUsageV2> randomAddressUsages =
                CreateRandomEventAddressUsageV2s();

            var returnedHealthReport = new HealthReportV2
            {
                AddressUsage = randomAddressUsages
            };

            IReadOnlyList<EventAddressUsageV2> expectedAddressUsages =
                randomAddressUsages.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveAddressUsageReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            IReadOnlyList<EventAddressUsageV2> actualAddressUsages =
                await this.healthAddressClientV2
                    .RetrieveEventAddressSummaryV2Async(
                        randomPeriod, randomWindowStart, null, randomCancellationToken);

            // then
            actualAddressUsages.Should()
                .BeEquivalentTo(expectedAddressUsages);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveAddressUsageReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventAddressSummaryV2ForCustomPeriodAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = TrafficPeriodV2.Custom;
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();
            DateTimeOffset randomWindowEnd = randomWindowStart.AddDays(5);

            IReadOnlyList<EventAddressUsageV2> randomAddressUsages =
                CreateRandomEventAddressUsageV2s();

            var returnedHealthReport = new HealthReportV2
            {
                AddressUsage = randomAddressUsages
            };

            IReadOnlyList<EventAddressUsageV2> expectedAddressUsages =
                randomAddressUsages.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveAddressUsageReportV2Async(
                    inputPeriod, randomWindowStart, randomWindowEnd, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            IReadOnlyList<EventAddressUsageV2> actualAddressUsages =
                await this.healthAddressClientV2
                    .RetrieveEventAddressSummaryV2Async(
                        inputPeriod, randomWindowStart, randomWindowEnd, randomCancellationToken);

            // then
            actualAddressUsages.Should()
                .BeEquivalentTo(expectedAddressUsages);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveAddressUsageReportV2Async(
                    inputPeriod, randomWindowStart, randomWindowEnd, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
