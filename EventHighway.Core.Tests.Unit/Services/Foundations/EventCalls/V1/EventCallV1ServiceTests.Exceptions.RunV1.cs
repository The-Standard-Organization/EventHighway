// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;
using FluentAssertions;
using Moq;
using RESTFulSense.Exceptions;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V1
{
    public partial class EventCallV1ServiceTests
    {
        [Theory]
        [MemberData(nameof(CriticalDependencyExceptions))]
        public async Task ShouldThrowCriticalDependencyExceptionOnRunV1IfCriticalDependencyErrorOccursAndLogItAsync(
            Xeption criticalDependencyException)
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var failedConfigurationEventCallV1Exception =
                new FailedConfigurationEventCallV1Exception(
                    message: "Failed event call configuration error occurred, contact support.",
                    innerException: criticalDependencyException,
                    data: criticalDependencyException.Data);

            var expectedEventCallV1DependencyException =
                new EventCallV1DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: failedConfigurationEventCallV1Exception);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(criticalDependencyException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyException actualEventCallV1DependencyException =
                await Assert.ThrowsAsync<EventCallV1DependencyException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddV1IfHttpUnprocessableErrorOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var httpUnprocessableEntityException = new HttpResponseUnprocessableEntityException();
            httpUnprocessableEntityException.Data.Add("ErrorCode", new List<string> { "UnprocessableEntity" });

            var failedRequestEventCallV1Exception =
                new FailedRequestEventCallV1Exception(
                    message: "Failed event call request error occurred, fix the errors and try again.",
                    innerException: httpUnprocessableEntityException,
                    data: httpUnprocessableEntityException.Data);

            var expectedEventCallV1DependencyValidationException =
                new EventCallV1DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: failedRequestEventCallV1Exception);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpUnprocessableEntityException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyValidationException actualEventCallV1DependencyValidationException =
                await Assert.ThrowsAsync<EventCallV1DependencyValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyValidationException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddV1IfBadRequestErrorOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            HttpResponseBadRequestException httpBadRequestException =
                CreateHttpBadRequestException();

            var invalidEventCallV1Exception =
                new InvalidEventCallV1Exception(
                    message: "Event call is invalid, fix the errors and try again.",
                    innerException: httpBadRequestException,
                    data: httpBadRequestException.Data);

            var expectedEventCallV1DependencyValidationException =
                new EventCallV1DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV1Exception);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpBadRequestException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyValidationException actualEventCallV1DependencyValidationException =
                await Assert.ThrowsAsync<EventCallV1DependencyValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyValidationException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddV1IfHttpConflictErrorOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var httpConflictException = new HttpResponseConflictException();
            httpConflictException.Data.Add("ErrorCode", new List<string> { "Conflict" });

            var alreadyExistsEventCallV1Exception =
                new AlreadyExistsEventCallV1Exception(
                    message: "Event call with same id already exists, try again.",
                    innerException: httpConflictException,
                    data: httpConflictException.Data);

            var expectedEventCallV1DependencyValidationException =
                new EventCallV1DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventCallV1Exception);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpConflictException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyValidationException actualEventCallV1DependencyValidationException =
                await Assert.ThrowsAsync<EventCallV1DependencyValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyValidationException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddV1IfFailedDependencyErrorOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var httpResponseFailedDependencyException =
                new HttpResponseFailedDependencyException();

            var invalidReferenceEventCallV1Exception =
                new InvalidReferenceEventCallV1Exception(
                    message: "Invalid event call reference error occurred, fix the errors and try again.",
                    innerException: httpResponseFailedDependencyException,
                    data: httpResponseFailedDependencyException.Data);

            var expectedEventCallV1DependencyValidationException =
                new EventCallV1DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceEventCallV1Exception);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpResponseFailedDependencyException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyValidationException actualEventCallV1DependencyValidationException =
                await Assert.ThrowsAsync<EventCallV1DependencyValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyValidationException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddV1IfHttpErrorOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var httpException = new HttpResponseException();
            httpException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventCallV1DependencyException =
                new FailedEventCallV1DependencyException(
                    message: "Failed event call dependency error occurred, contact support.",
                    innerException: httpException,
                    data: httpException.Data);

            var expectedEventCallV1DependencyException =
                new EventCallV1DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: failedEventCallV1DependencyException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyException actualEventCallV1DependencyException =
                await Assert.ThrowsAsync<EventCallV1DependencyException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddV1IfExceptionOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventCallV1ServiceException =
                new FailedEventCallV1ServiceException(
                    message: "Failed event call service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventCallV1ServiceException =
                new EventCallV1ServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: failedEventCallV1ServiceException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1ServiceException actualEventCallV1ServiceException =
                await Assert.ThrowsAsync<EventCallV1ServiceException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ServiceException.Should()
                .BeEquivalentTo(expectedEventCallV1ServiceException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ServiceException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
