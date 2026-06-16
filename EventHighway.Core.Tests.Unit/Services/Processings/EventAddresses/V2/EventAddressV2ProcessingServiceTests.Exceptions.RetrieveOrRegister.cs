// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V2
{
    public partial class EventAddressV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRetrieveOrRegisterIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 =
                CreateRandomEventAddressV2();

            var expectedEventAddressV2ProcessingDependencyValidationException =
                new EventAddressV2ProcessingDependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV2> retrieveOrRegisterEventAddressV2Task =
                this.eventAddressV2ProcessingService.RetrieveOrRegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ProcessingDependencyValidationException
                actualEventAddressV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyValidationException>(
                        retrieveOrRegisterEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingDependencyValidationException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveOrRegisterIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 =
                CreateRandomEventAddressV2();

            var expectedEventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventAddressV2> retrieveOrRegisterEventAddressV2Task =
                this.eventAddressV2ProcessingService.RetrieveOrRegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ProcessingDependencyException
                actualEventAddressV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyException>(
                        retrieveOrRegisterEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveOrRegisterIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 =
                CreateRandomEventAddressV2();

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventAddressV2ProcessingServiceException =
                new FailedEventAddressV2ProcessingServiceException(
                    message: "Failed event address service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventAddressV2ProcessingServiceException =
                new EventAddressV2ProcessingServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: failedEventAddressV2ProcessingServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV2> retrieveOrRegisterEventAddressV2Task =
                this.eventAddressV2ProcessingService.RetrieveOrRegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ProcessingServiceException
                actualEventAddressV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingServiceException>(
                        retrieveOrRegisterEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
