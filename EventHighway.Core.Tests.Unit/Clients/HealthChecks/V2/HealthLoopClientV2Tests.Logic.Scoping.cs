// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthLoopClientV2Tests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            TrafficPeriodV2 period = GetRandomTrafficPeriodV2();
            DateTimeOffset windowStart = GetRandomDateTimeOffset();
            int expectedResolutionCount = 2;

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new HealthReportV2());

            // when
            await this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(period, windowStart);
            await this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(period, windowStart);

            // then
            this.coordinationServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
