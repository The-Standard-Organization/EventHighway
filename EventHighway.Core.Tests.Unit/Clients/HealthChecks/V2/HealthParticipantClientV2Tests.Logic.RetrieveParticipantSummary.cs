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
    public partial class HealthParticipantClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveParticipantSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            IReadOnlyList<ParticipantUsageV2> randomParticipantUsages =
                CreateRandomParticipantUsageV2s();

            var returnedHealthReport = new HealthReportV2
            {
                ParticipantUsage = randomParticipantUsages
            };

            IReadOnlyList<ParticipantUsageV2> expectedParticipantUsages =
                randomParticipantUsages.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken))
                        .ReturnsAsync(returnedHealthReport);

            // when
            IReadOnlyList<ParticipantUsageV2> actualParticipantUsages =
                await this.healthParticipantClientV2
                    .RetrieveParticipantSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualParticipantUsages.Should()
                .BeEquivalentTo(expectedParticipantUsages);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
