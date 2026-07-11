// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

            var expectedEventHandlerV2ClientValidationException =
                new EventHandlerV2ClientValidationException(
                    message: "Event handler client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ClientValidationException actualEventHandlerV2ClientValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ClientValidationException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ClientValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientValidationException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventHandlerV2ClientDependencyException =
                new EventHandlerV2ClientDependencyException(
                    message: "Event handler client dependency error occurred, contact support.",
                    innerException: eventHandlerV2ProcessingDependencyException.InnerException as Xeption,
                    data: (eventHandlerV2ProcessingDependencyException.InnerException as Xeption).Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventHandlerV2ProcessingDependencyException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ClientDependencyException actualEventHandlerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventHandlerV2ClientDependencyException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ClientDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientDependencyException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventHandlerV2ProcessingServiceException =
                new EventHandlerV2ProcessingServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventHandlerV2ClientDependencyException =
                new EventHandlerV2ClientDependencyException(
                    message: "Event handler client dependency error occurred, contact support.",
                    innerException: eventHandlerV2ProcessingServiceException.InnerException as Xeption,
                    data: (eventHandlerV2ProcessingServiceException.InnerException as Xeption).Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventHandlerV2ProcessingServiceException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ClientDependencyException actualEventHandlerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventHandlerV2ClientDependencyException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ClientDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientDependencyException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();
            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventHandlerV2ClientServiceException =
                new EventHandlerV2ClientServiceException(
                    message: "Event handler client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ClientServiceException actualEventHandlerV2ClientServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ClientServiceException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ClientServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientServiceException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            OperationCanceledException actualOperationCanceledException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualOperationCanceledException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
