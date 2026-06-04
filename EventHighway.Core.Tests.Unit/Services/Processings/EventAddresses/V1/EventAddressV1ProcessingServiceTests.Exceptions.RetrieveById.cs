// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V1
{
    public partial class EventAddressV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRetrieveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();

            var expectedEventAddressV1ProcessingDependencyValidationException =
                new EventAddressV1ProcessingDependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV1> retrieveEventAddressV1ByIdTask =
                this.eventAddressV1ProcessingService.RetrieveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1ProcessingDependencyValidationException
                actualEventAddressV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventAddressV1ProcessingDependencyValidationException>(
                        retrieveEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1ProcessingDependencyValidationException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();

            var expectedEventAddressV1ProcessingDependencyException =
                new EventAddressV1ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventAddressV1> retrieveEventAddressV1ByIdTask =
                this.eventAddressV1ProcessingService.RetrieveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1ProcessingDependencyException
                actualEventAddressV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventAddressV1ProcessingDependencyException>(
                        retrieveEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1ProcessingDependencyException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ProcessingDependencyException))),
                        Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventAddressV1ProcessingServiceException =
                new FailedEventAddressV1ProcessingServiceException(
                    message: "Failed event address service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventAddressV1ProcessingExceptionException =
                new EventAddressV1ProcessingServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: failedEventAddressV1ProcessingServiceException);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV1> retrieveEventAddressV1ByIdTask =
                this.eventAddressV1ProcessingService.RetrieveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1ProcessingServiceException
                actualEventAddressV1ProcessingServiceException =
                    await Assert.ThrowsAsync<EventAddressV1ProcessingServiceException>(
                        retrieveEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV1ProcessingExceptionException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ProcessingExceptionException))),
                        Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
