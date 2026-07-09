// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventParticipants.V2
{
    public partial class EventParticipantV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfTimeoutOccursAndLogItAsync()
        {
            // given
            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventParticipantV2ProcessingException =
                new TimeoutEventParticipantV2ProcessingException(
                    message: "Failed event participant processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventParticipantV2ProcessingDependencyException =
                new EventParticipantV2ProcessingDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: timeoutEventParticipantV2ProcessingException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2ProcessingService.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    TestContext.Current.CancellationToken);

            EventParticipantV2ProcessingDependencyException actualEventParticipantV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventParticipantV2ProcessingDependencyException>(
                    addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventParticipantV2ProcessingDependencyException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingDependencyException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnAddAsync()
        {
            // given
            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2ProcessingService.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    addEventParticipantV2Task.AsTask);

            actualException.Should().NotBeOfType<EventParticipantV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventParticipantV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();

            var expectedEventParticipantV2ProcessingDependencyValidationException =
                new EventParticipantV2ProcessingDependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2ProcessingService.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ProcessingDependencyValidationException
                actualEventParticipantV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingDependencyValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventParticipantV2ProcessingDependencyValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();

            var expectedEventParticipantV2ProcessingDependencyException =
                new EventParticipantV2ProcessingDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2ProcessingService.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ProcessingDependencyException
                actualEventParticipantV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingDependencyException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventParticipantV2ProcessingDependencyException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingDependencyException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventParticipantV2ProcessingServiceException =
                new FailedEventParticipantV2ProcessingServiceException(
                    message: "Failed event participant service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventParticipantV2ProcessingServiceException =
                new EventParticipantV2ProcessingServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: failedEventParticipantV2ProcessingServiceException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2ProcessingService.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ProcessingServiceException
                actualEventParticipantV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingServiceException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ProcessingServiceException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingServiceException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
