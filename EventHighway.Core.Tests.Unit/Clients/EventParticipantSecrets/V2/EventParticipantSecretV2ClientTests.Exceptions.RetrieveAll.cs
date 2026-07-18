// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
            ShouldThrowValidationExceptionOnRetrieveAllIfValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2ValidationException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveAllEventParticipantSecretV2sTask =
                this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                    new EventParticipantSecretV2Query(), randomCancellationToken);

            EventParticipantSecretV2ClientValidationException
                actualEventParticipantSecretV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientValidationException>(
                        retrieveAllEventParticipantSecretV2sTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientValidationException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRetrieveAllIfDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2DependencyValidationException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveAllEventParticipantSecretV2sTask =
                this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                    new EventParticipantSecretV2Query(), randomCancellationToken);

            EventParticipantSecretV2ClientValidationException
                actualEventParticipantSecretV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientValidationException>(
                        retrieveAllEventParticipantSecretV2sTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientValidationException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2DependencyException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveAllEventParticipantSecretV2sTask =
                this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                    new EventParticipantSecretV2Query(), randomCancellationToken);

            EventParticipantSecretV2ClientDependencyException
                actualEventParticipantSecretV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientDependencyException>(
                        retrieveAllEventParticipantSecretV2sTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientDependencyException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveAllIfServiceExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken))
                        .ThrowsAsync(eventParticipantSecretV2ServiceException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveAllEventParticipantSecretV2sTask =
                this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                    new EventParticipantSecretV2Query(), randomCancellationToken);

            EventParticipantSecretV2ClientDependencyException
                actualEventParticipantSecretV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientDependencyException>(
                        retrieveAllEventParticipantSecretV2sTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientDependencyException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someException = new Exception(message: GetRandomString());

            var expectedEventParticipantSecretV2ClientServiceException =
                new EventParticipantSecretV2ClientServiceException(
                    message: "Event participant secret client service error occurred, contact support.",
                    innerException: new Xeption(someException.Message, someException),
                    data: someException.Data);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken))
                        .ThrowsAsync(someException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveAllEventParticipantSecretV2sTask =
                this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                    new EventParticipantSecretV2Query(), randomCancellationToken);

            EventParticipantSecretV2ClientServiceException
                actualEventParticipantSecretV2ClientServiceException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ClientServiceException>(
                        retrieveAllEventParticipantSecretV2sTask.AsTask);

            // then
            actualEventParticipantSecretV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ClientServiceException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveAllEventParticipantSecretV2sTask =
                this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                    new EventParticipantSecretV2Query(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllEventParticipantSecretV2sTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    It.IsAny<EventParticipantSecretV2Query>(), randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
