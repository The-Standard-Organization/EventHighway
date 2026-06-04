// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V1
{
    public partial class ListenerEventV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ListenerEventV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption listenerEventV1ValidationException)
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();

            var expectedListenerEventV1ProcessingDependencyValidationException =
                new ListenerEventV1ProcessingDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: listenerEventV1ValidationException.InnerException as Xeption);

            this.listenerEventV1ServiceMock.Setup(service =>
                service.AddListenerEventV1Async(It.IsAny<ListenerEventV1>()))
                    .ThrowsAsync(listenerEventV1ValidationException);

            // when
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1ProcessingService.AddListenerEventV1Async(someListenerEventV1);

            ListenerEventV1ProcessingDependencyValidationException
                actualListenerEventV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingDependencyValidationException>(
                        addListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ProcessingDependencyValidationException);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.AddListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ListenerEventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption listenerEventV1DependencyException)
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();

            var expectedListenerEventV1ProcessingDependencyException =
                new ListenerEventV1ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: listenerEventV1DependencyException.InnerException as Xeption);

            this.listenerEventV1ServiceMock.Setup(service =>
                service.AddListenerEventV1Async(It.IsAny<ListenerEventV1>()))
                    .ThrowsAsync(listenerEventV1DependencyException);

            // when
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1ProcessingService.AddListenerEventV1Async(someListenerEventV1);

            ListenerEventV1ProcessingDependencyException
                actualListenerEventV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingDependencyException>(
                        addListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ProcessingDependencyException);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.AddListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedListenerEventV1ProcessingServiceException =
                new FailedListenerEventV1ProcessingServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV1ProcessingExceptionException =
                new ListenerEventV1ProcessingServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV1ProcessingServiceException);

            this.listenerEventV1ServiceMock.Setup(service =>
                service.AddListenerEventV1Async(It.IsAny<ListenerEventV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1ProcessingService.AddListenerEventV1Async(
                    someListenerEventV1);

            ListenerEventV1ProcessingServiceException
                actualListenerEventV1ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingServiceException>(
                        addListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ProcessingExceptionException);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.AddListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingExceptionException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
