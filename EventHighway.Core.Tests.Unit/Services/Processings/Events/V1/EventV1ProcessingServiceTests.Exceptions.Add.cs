// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V1
{
    public partial class EventV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1ProcessingDependencyValidationException =
                new EventV1ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ServiceMock.Setup(service =>
                service.AddEventV1Async(It.IsAny<EventV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1ProcessingService.AddEventV1Async(someEventV1);

            EventV1ProcessingDependencyValidationException
                actualEventV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ProcessingDependencyValidationException>(
                        addEventV1Task.AsTask);

            // then
            actualEventV1ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingDependencyValidationException);

            this.eventV1ServiceMock.Verify(service =>
                service.AddEventV1Async(It.IsAny<EventV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1ProcessingDependencyException =
                new EventV1ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1ServiceMock.Setup(service =>
                service.AddEventV1Async(It.IsAny<EventV1>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1ProcessingService.AddEventV1Async(someEventV1);

            EventV1ProcessingDependencyException
                actualEventV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventV1ProcessingDependencyException>(
                        addEventV1Task.AsTask);

            // then
            actualEventV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingDependencyException);

            this.eventV1ServiceMock.Verify(service =>
                service.AddEventV1Async(It.IsAny<EventV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingDependencyException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV1ProcessingServiceException =
                new FailedEventV1ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV1ProcessingExceptionException =
                new EventV1ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1ProcessingServiceException);

            this.eventV1ServiceMock.Setup(service =>
                service.AddEventV1Async(It.IsAny<EventV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1ProcessingService.AddEventV1Async(
                    someEventV1);

            EventV1ProcessingServiceException
                actualEventV1ProcessingServiceException =
                    await Assert.ThrowsAsync<EventV1ProcessingServiceException>(
                        addEventV1Task.AsTask);

            // then
            actualEventV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingExceptionException);

            this.eventV1ServiceMock.Verify(service =>
                service.AddEventV1Async(It.IsAny<EventV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingExceptionException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
