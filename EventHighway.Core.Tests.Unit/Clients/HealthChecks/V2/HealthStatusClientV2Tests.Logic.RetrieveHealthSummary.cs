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
    public partial class HealthStatusClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveHealthRagStatusV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            IReadOnlyList<HealthCheckItemV2> randomHealthCheckItemV2s =
                CreateRandomHealthCheckItemV2s();

            var returnedHealthReport = new HealthReportV2
            {
                HealthCheckItems = randomHealthCheckItemV2s
            };

            IReadOnlyList<HealthCheckItemV2> expectedHealthCheckItemV2s =
                randomHealthCheckItemV2s.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            IReadOnlyList<HealthCheckItemV2> actualHealthCheckItemV2s =
                await this.healthV2Client
                    .RetrieveHealthRagStatusV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthCheckItemV2s.Should()
                .BeEquivalentTo(expectedHealthCheckItemV2s);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
