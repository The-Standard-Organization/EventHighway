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
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();

            var expectedEventAddressV2ProcessingDependencyValidationException =
                new EventAddressV2ProcessingDependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ProcessingDependencyValidationException
                actualEventAddressV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyValidationException>(
                        removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingDependencyValidationException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
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
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();

            var expectedEventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ProcessingDependencyException
                actualEventAddressV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyException>(
                        removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();

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
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ProcessingServiceException
                actualEventAddressV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingServiceException>(
                        removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
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
