// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Processings.EventCalls.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V1
{
    public partial class EventCallV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(EventCallV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRunV1IfDependencyValidationErrorAndLogItAsync(
            Xeption eventCallV1ValidationException)
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var expectedEventCallV1ProcessingDependencyValidationException =
                new EventCallV1ProcessingDependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: eventCallV1ValidationException.InnerException as Xeption);

            this.eventCallV1ServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()))
                    .ThrowsAsync(eventCallV1ValidationException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1ProcessingService.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1ProcessingDependencyValidationException
                actualEventCallV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventCallV1ProcessingDependencyValidationException>(
                        runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV1ProcessingDependencyValidationException);

            this.eventCallV1ServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventCallV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventCallV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRunV1IfDependencyExceptionOccursAndLogItAsync(
            Xeption eventCallV1DependencyException)
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var expectedEventCallV1ProcessingDependencyException =
                new EventCallV1ProcessingDependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: eventCallV1DependencyException.InnerException as Xeption);

            this.eventCallV1ServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()))
                    .ThrowsAsync(eventCallV1DependencyException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1ProcessingService.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1ProcessingDependencyException
                actualEventCallV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventCallV1ProcessingDependencyException>(
                        runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventCallV1ProcessingDependencyException);

            this.eventCallV1ServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ProcessingDependencyException))),
                        Times.Once);

            this.eventCallV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRunV1IfExceptionOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var serviceException = new Exception();

            var failedEventCallV1ProcessingServiceException =
                new FailedEventCallV1ProcessingServiceException(
                    message: "Failed event call service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventCallV1ProcessingExceptionException =
                new EventCallV1ProcessingServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: failedEventCallV1ProcessingServiceException);

            this.eventCallV1ServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1ProcessingService.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1ProcessingServiceException
                actualEventCallV1ProcessingServiceException =
                    await Assert.ThrowsAsync<EventCallV1ProcessingServiceException>(
                        runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventCallV1ProcessingExceptionException);

            this.eventCallV1ServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(It.IsAny<EventCallV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ProcessingExceptionException))),
                        Times.Once);

            this.eventCallV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
