// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveOrRegisterIfEventHandlerV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler nullEventHandler = null;

            var nullEventHandlerV2ProcessingException =
                new NullEventHandlerV2ProcessingException(
                    message: "Event handler is null.");

            var expectedEventHandlerV2ProcessingValidationException =
                new EventHandlerV2ProcessingValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: nullEventHandlerV2ProcessingException);

            // when
            ValueTask<IEventHandler> retrieveOrRegisterEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RetrieveOrRegisterEventHandlerV2Async(
                    nullEventHandler, randomCancellationToken);

            EventHandlerV2ProcessingValidationException actualEventHandlerV2ProcessingValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ProcessingValidationException>(
                    retrieveOrRegisterEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingValidationException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveOrRegisterIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventHandlerMock = new Mock<IEventHandler>();
            invalidEventHandlerMock.SetupGet(eventHandler => eventHandler.Id).Returns(Guid.Empty);
            IEventHandler invalidEventHandler = invalidEventHandlerMock.Object;

            var invalidEventHandlerV2ProcessingException =
                new InvalidEventHandlerV2ProcessingException(
                    message: "Event handler is invalid, fix the errors and try again.");

            invalidEventHandlerV2ProcessingException.AddData(
                key: nameof(IEventHandler.Id),
                values: "Required");

            var expectedEventHandlerV2ProcessingValidationException =
                new EventHandlerV2ProcessingValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: invalidEventHandlerV2ProcessingException);

            // when
            ValueTask<IEventHandler> retrieveOrRegisterEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RetrieveOrRegisterEventHandlerV2Async(
                    invalidEventHandler, randomCancellationToken);

            EventHandlerV2ProcessingValidationException actualEventHandlerV2ProcessingValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ProcessingValidationException>(
                    retrieveOrRegisterEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingValidationException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
