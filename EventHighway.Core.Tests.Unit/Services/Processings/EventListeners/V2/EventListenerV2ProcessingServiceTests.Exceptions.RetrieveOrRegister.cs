// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V2
{
    public partial class EventListenerV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationOnRetrieveOrRegisterIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            EventListenerV2 someEventListenerV2 = CreateRandomEventListenerV2();

            var expectedEventListenerV2ProcessingDependencyValidationException =
                new EventListenerV2ProcessingDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                    someEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ProcessingDependencyValidationException
                actualEventListenerV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyValidationException>(
                        retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingDependencyValidationException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveOrRegisterIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            EventListenerV2 someEventListenerV2 = CreateRandomEventListenerV2();

            var expectedEventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                    someEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ProcessingDependencyException
                actualEventListenerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyException>(
                        retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingDependencyException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveOrRegisterIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            EventListenerV2 someEventListenerV2 = CreateRandomEventListenerV2();

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV2ProcessingServiceException =
                new FailedEventListenerV2ProcessingServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV2ProcessingServiceException =
                new EventListenerV2ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV2ProcessingServiceException);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                    someEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ProcessingServiceException
                actualEventListenerV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingServiceException>(
                        retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingServiceException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingServiceException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
