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
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            IReadOnlyList<EventAddressUsageV2> actualAddressUsages =
                await this.healthAddressClientV2
                    .RetrieveEventAddressSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualAddressUsages.Should()
                .BeEquivalentTo(expectedAddressUsages);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveAddressUsageReportV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
