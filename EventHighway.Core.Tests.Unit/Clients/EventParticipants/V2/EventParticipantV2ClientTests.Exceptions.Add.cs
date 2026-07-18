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
        public async Task ShouldThrowValidationExceptionOnAddIfValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingValidationException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnAddIfDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingDependencyValidationException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingDependencyException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientDependencyException
                actualEventParticipantV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientDependencyException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientDependencyException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnAddIfServiceExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ProcessingServiceException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientDependencyException
                actualEventParticipantV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientDependencyException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientDependencyException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
            var someException = new Exception(message: GetRandomString());

            var expectedEventParticipantV2ClientServiceException =
                new EventParticipantV2ClientServiceException(
                    message: "Event participant client service error occurred, contact support.",
                    innerException: new Xeption(someException.Message, someException),
                    data: someException.Data);

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(someException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientServiceException
                actualEventParticipantV2ClientServiceException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientServiceException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientServiceException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnAddAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    addEventParticipantV2Task.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
