// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnTryClaimScheduledIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventV2Id = Guid.Empty;

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Id),
                values: "Required");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            // when
            ValueTask<int> tryClaimTask =
                this.eventV2Service.TryClaimScheduledEventV2Async(
                    invalidEventV2Id, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    tryClaimTask.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.ClaimScheduledEventV2Async(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnTryClaimScheduledAsync()
        {
            // given
            Guid someEventV2Id = Guid.NewGuid();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<int> tryClaimTask =
                this.eventV2Service.TryClaimScheduledEventV2Async(
                    someEventV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    tryClaimTask.AsTask);

            actualException.Should().NotBeOfType<EventV2DependencyException>();
            actualException.Should().NotBeOfType<EventV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnTryClaimScheduledWhenTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = Guid.NewGuid();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2Exception =
                new TimeoutEventV2Exception(
                    message: "Failed event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.ClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<int> tryClaimTask =
                this.eventV2Service.TryClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    tryClaimTask.AsTask);

            // then
            actualEventV2DependencyException.Should().BeEquivalentTo(
                expectedEventV2DependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.ClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnTryClaimScheduledIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedStorageEventV2Exception =
                new FailedStorageEventV2Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.ClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<int> tryClaimTask =
                this.eventV2Service.TryClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    tryClaimTask.AsTask);

            // then
            actualEventV2DependencyException.Should().BeEquivalentTo(
                expectedEventV2DependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.ClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnTryClaimScheduledIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = Guid.NewGuid();
            var serviceException = new Exception();

            var failedEventV2ServiceException =
                new FailedEventV2ServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ServiceException =
                new EventV2ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.ClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<int> tryClaimTask =
                this.eventV2Service.TryClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken);

            EventV2ServiceException actualEventV2ServiceException =
                await Assert.ThrowsAsync<EventV2ServiceException>(
                    tryClaimTask.AsTask);

            // then
            actualEventV2ServiceException.Should().BeEquivalentTo(
                expectedEventV2ServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.ClaimScheduledEventV2Async(
                    someEventV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
