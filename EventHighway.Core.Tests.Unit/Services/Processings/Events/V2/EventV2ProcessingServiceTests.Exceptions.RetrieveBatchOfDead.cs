// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfDeadIfDependencyErrorOccursAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            var expectedEventV2ProcessingDependencyException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ThrowsAsync(eventV2DependencyException);

            // when
            var retrieveBatchOfDeadTask =
                this.eventV2ProcessingService
                    .RetrieveBatchOfDeadEventV2sAsync(inputTake);

            EventV2ProcessingDependencyException actualEventV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                    retrieveBatchOfDeadTask.AsTask);

            // then
            actualEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOfDeadIfValidationErrorOccursAndLogItAsync(
            Xeption eventV2ValidationException)
        {
            // given
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            var expectedEventV2ProcessingDependencyValidationException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: eventV2ValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ThrowsAsync(eventV2ValidationException);

            // when
            var retrieveBatchOfDeadTask =
                this.eventV2ProcessingService
                    .RetrieveBatchOfDeadEventV2sAsync(inputTake);

            EventV2ProcessingDependencyValidationException actualEventV2ProcessingDependencyValidationException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                    retrieveBatchOfDeadTask.AsTask);

            // then
            actualEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyValidationException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOfDeadIfExceptionOccursAndLogItAsync()
        {
            // given
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;
            var serviceException = new Exception();

            var failedEventV2ProcessingServiceException =
                new FailedEventV2ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ProcessingServiceException =
                new EventV2ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ProcessingServiceException);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ThrowsAsync(serviceException);

            // when
            var retrieveBatchOfDeadTask =
                this.eventV2ProcessingService
                    .RetrieveBatchOfDeadEventV2sAsync(inputTake);

            EventV2ProcessingServiceException actualEventV2ProcessingServiceException =
                await Assert.ThrowsAsync<EventV2ProcessingServiceException>(
                    retrieveBatchOfDeadTask.AsTask);

            // then
            actualEventV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingServiceException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingServiceException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
