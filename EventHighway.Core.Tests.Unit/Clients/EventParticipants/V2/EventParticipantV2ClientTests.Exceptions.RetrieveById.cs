// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRetrieveByIdIfValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2ProcessingValidationException =
                new EventParticipantV2ProcessingValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientValidationException =
                new EventParticipantV2ClientValidationException(
                    message: "Event participant client validation error occurred, fix the errors and try again.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingValidationException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRetrieveByIdIfDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2ProcessingDependencyValidationException =
                new EventParticipantV2ProcessingDependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientValidationException =
                new EventParticipantV2ClientValidationException(
                    message: "Event participant client validation error occurred, fix the errors and try again.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingDependencyValidationException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveByIdIfDependencyExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2ProcessingDependencyException =
                new EventParticipantV2ProcessingDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientDependencyException =
                new EventParticipantV2ClientDependencyException(
                    message: "Event participant client dependency error occurred, contact support.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingDependencyException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken);

            EventParticipantV2ClientDependencyException
                actualEventParticipantV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientDependencyException>(
                        retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientDependencyException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveByIdIfServiceExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2ProcessingServiceException =
                new EventParticipantV2ProcessingServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientDependencyException =
                new EventParticipantV2ClientDependencyException(
                    message: "Event participant client dependency error occurred, contact support.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingServiceException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken);

            EventParticipantV2ClientDependencyException
                actualEventParticipantV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientDependencyException>(
                        retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientDependencyException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            var someException = new Exception(message: GetRandomString());

            var expectedEventParticipantV2ClientServiceException =
                new EventParticipantV2ClientServiceException(
                    message: "Event participant client service error occurred, contact support.",
                    innerException: someException as Xeption,
                    data: someException.Data);

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(someException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken);

            EventParticipantV2ClientServiceException
                actualEventParticipantV2ClientServiceException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientServiceException>(
                        retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientServiceException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
