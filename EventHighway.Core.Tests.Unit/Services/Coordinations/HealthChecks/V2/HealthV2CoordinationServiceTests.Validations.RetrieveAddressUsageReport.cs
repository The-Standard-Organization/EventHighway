// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveAddressUsageReportV2IfWindowStartIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset invalidWindowStart = default;

            var invalidHealthV2CoordinationException =
                new InvalidHealthV2CoordinationException(
                    message: "Health coordination is invalid, fix the errors and try again.");

            invalidHealthV2CoordinationException.UpsertDataList(
                key: "WindowStart",
                value: "Required");

            var expectedHealthV2CoordinationValidationException =
                new HealthV2CoordinationValidationException(
                    message: "Health coordination validation error occurred, fix the errors and try again.",
                    innerException: invalidHealthV2CoordinationException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, invalidWindowStart, null, randomCancellationToken);

            HealthV2CoordinationValidationException actualHealthV2CoordinationValidationException =
                await Assert.ThrowsAsync<HealthV2CoordinationValidationException>(
                    retrieveHealthReportTask.AsTask);

            // then
            actualHealthV2CoordinationValidationException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationValidationException))),
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
