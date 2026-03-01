// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventCallV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRunV1IfEventCallV1ValidationErrorOccursAndLogItAsync(
            Xeption eventCallV1ValidationException)
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var expectedEventV1OrchestrationDependencyValidationException =
                new EventV1OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: eventCallV1ValidationException.InnerException as Xeption);

            this.eventCallV1ProcessingServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()))
                    .ThrowsAsync(eventCallV1ValidationException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventV1OrchestrationService.RunEventCallV1AsyncV1(
                    someEventCallV1);

            EventV1OrchestrationDependencyValidationException
                actualEventV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationDependencyValidationException>(
                        runEventCallV1Task.AsTask);

            // then
            actualEventV1OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyValidationException);

            this.eventCallV1ProcessingServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventCallV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRunV1IfEventCallV1DependencyErrorOccursAndLogItAsync(
            Xeption eventCallV1DependencyException)
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var expectedEventV1OrchestrationDependencyException =
                new EventV1OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventCallV1DependencyException.InnerException as Xeption);

            this.eventCallV1ProcessingServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()))
                    .ThrowsAsync(eventCallV1DependencyException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventV1OrchestrationService.RunEventCallV1AsyncV1(
                    someEventCallV1);

            EventV1OrchestrationDependencyException actualEventV1OrchestrationDependencyException =
                await Assert.ThrowsAsync<EventV1OrchestrationDependencyException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyException);

            this.eventCallV1ProcessingServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyException))),
                        Times.Once);

            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRunV1IfExceptionOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var serviceException = new Exception();

            var failedEventV1OrchestrationServiceException =
                new FailedEventV1OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1OrchestrationServiceException =
                new EventV1OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1OrchestrationServiceException);

            this.eventCallV1ProcessingServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventV1OrchestrationService.RunEventCallV1AsyncV1(
                    someEventCallV1);

            EventV1OrchestrationServiceException actualEventV1OrchestrationServiceException =
                await Assert.ThrowsAsync<EventV1OrchestrationServiceException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationServiceException);

            this.eventCallV1ProcessingServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationServiceException))),
                        Times.Once);

            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
