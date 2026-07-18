// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipantSecrets.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRemoveByIdIfValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: someInnerException);

            var expectedEventParticipantSecretV2ClientValidationException =
                new EventParticipantSecretV2ClientValidationException(
                    message: "Event participant secret client validation error occurred, fix the errors and try again.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2ValidationException);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Client.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken);

            EventParticipantSecretV2ClientValidationException
                actualEventParticipantSecretV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientValidationException>(
                        removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientValidationException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRemoveByIdIfDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantSecretV2DependencyValidationException =
                new EventParticipantSecretV2DependencyValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: someInnerException);

            var expectedEventParticipantSecretV2ClientValidationException =
                new EventParticipantSecretV2ClientValidationException(
                    message: "Event participant secret client validation error occurred, fix the errors and try again.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2DependencyValidationException);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Client.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken);

            EventParticipantSecretV2ClientValidationException
                actualEventParticipantSecretV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientValidationException>(
                        removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientValidationException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: someInnerException);

            var expectedEventParticipantSecretV2ClientDependencyException =
                new EventParticipantSecretV2ClientDependencyException(
                    message: "Event participant secret client dependency error occurred, contact support.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2DependencyException);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Client.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken);

            EventParticipantSecretV2ClientDependencyException
                actualEventParticipantSecretV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientDependencyException>(
                        removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientDependencyException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRemoveByIdIfServiceExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantSecretV2ServiceException =
                new EventParticipantSecretV2ServiceException(
                    message: "Event participant secret service error occurred, contact support.",
                    innerException: someInnerException);

            var expectedEventParticipantSecretV2ClientDependencyException =
                new EventParticipantSecretV2ClientDependencyException(
                    message: "Event participant secret client dependency error occurred, contact support.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2ServiceException);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Client.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken);

            EventParticipantSecretV2ClientDependencyException
                actualEventParticipantSecretV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientDependencyException>(
                        removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientDependencyException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();
            var someException = new Exception(message: GetRandomString());

            var expectedEventParticipantSecretV2ClientServiceException =
                new EventParticipantSecretV2ClientServiceException(
                    message: "Event participant secret client service error occurred, contact support.",
                    innerException: new Xeption(someException.Message, someException),
                    data: someException.Data);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(someException);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Client.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken);

            EventParticipantSecretV2ClientServiceException
                actualEventParticipantSecretV2ClientServiceException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientServiceException>(
                        removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientServiceException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Client.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
