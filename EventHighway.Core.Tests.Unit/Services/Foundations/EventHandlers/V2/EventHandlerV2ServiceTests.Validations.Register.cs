// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRegisterIfEventHandlerV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler nullEventHandler = null;

            var nullEventHandlerV2Exception =
                new NullEventHandlerV2Exception(
                    message: "Event handler is null.");

            var expectedEventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: nullEventHandlerV2Exception);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Service.RegisterEventHandlerV2Async(
                    nullEventHandler, randomCancellationToken);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ValidationException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ValidationException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRegisterIfEventHandlerV2IsInvalidAndLogItAsync(
            string invalidName)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventHandlerMock = new Mock<IEventHandler>();
            invalidEventHandlerMock.SetupGet(eventHandler => eventHandler.Id).Returns(Guid.Empty);
            invalidEventHandlerMock.SetupGet(eventHandler => eventHandler.Name).Returns(invalidName);
            IEventHandler invalidEventHandler = invalidEventHandlerMock.Object;

            var invalidEventHandlerV2Exception =
                new InvalidEventHandlerV2Exception(
                    message: "Event handler is invalid, fix the errors and try again.");

            invalidEventHandlerV2Exception.AddData(
                key: nameof(IEventHandler.Id),
                values: "Id required");

            invalidEventHandlerV2Exception.AddData(
                key: nameof(IEventHandler.Name),
                values: "Text required");

            var expectedEventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: invalidEventHandlerV2Exception);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Service.RegisterEventHandlerV2Async(
                    invalidEventHandler, randomCancellationToken);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ValidationException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ValidationException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
