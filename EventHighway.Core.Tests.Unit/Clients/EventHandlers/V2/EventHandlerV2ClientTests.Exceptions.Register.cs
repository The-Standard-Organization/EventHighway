// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
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
        public async Task ShouldThrowValidationExceptionOnRegisterIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

            var expectedEventHandlerV2ClientValidationException =
                new EventHandlerV2ClientValidationException(
                    message: "Event handler client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Client.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ClientValidationException actualEventHandlerV2ClientValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ClientValidationException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ClientValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientValidationException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRegisterIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();
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
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventHandlerV2ProcessingDependencyException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Client.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ClientDependencyException actualEventHandlerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventHandlerV2ClientDependencyException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ClientDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientDependencyException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRegisterIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();
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
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventHandlerV2ProcessingServiceException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Client.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ClientDependencyException actualEventHandlerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventHandlerV2ClientDependencyException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ClientDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientDependencyException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRegisterIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();
            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventHandlerV2ClientServiceException =
                new EventHandlerV2ClientServiceException(
                    message: "Event handler client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Client.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ClientServiceException actualEventHandlerV2ClientServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ClientServiceException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ClientServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientServiceException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRegisterAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Client.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            OperationCanceledException actualOperationCanceledException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualOperationCanceledException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
