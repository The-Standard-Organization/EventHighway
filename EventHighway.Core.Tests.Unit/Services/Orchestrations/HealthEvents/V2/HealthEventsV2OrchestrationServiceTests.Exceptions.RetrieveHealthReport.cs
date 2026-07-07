// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.HealthEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthEvents.V2
{
    public partial class HealthEventsV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveHealthReportV2IfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthEventsV2OrchestrationDependencyValidationException =
                new HealthEventsV2OrchestrationDependencyValidationException(
                    message: "Health events validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthEventsV2OrchestrationDependencyValidationException
                actualHealthEventsV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<HealthEventsV2OrchestrationDependencyValidationException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthEventsV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHealthEventsV2OrchestrationDependencyValidationException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthEventsV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
