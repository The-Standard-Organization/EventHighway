// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunIfEventCallV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventCallV2 nullEventCallV2 = null;

            var nullEventCallV2Exception =
                new NullEventCallV2Exception(message: "Event call is null.");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: nullEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(nullEventCallV2, randomCancellationToken);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRunIfEventCallV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventCallV2 = new EventCallV2
            {
                HandlerName = invalidText,
                Content = invalidText
            };

            var invalidEventCallV2Exception =
                new InvalidEventCallV2Exception(
                    message: "Event call is invalid, fix the errors and try again.");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.HandlerId),
                values: "Id required");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.HandlerName),
                values: "Text required");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.Content),
                values: "Payload required");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(invalidEventCallV2, randomCancellationToken);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunIfHandlerBrokerIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();

            IEventCallV2Service serviceWithNullBroker = new EventCallV2Service(
                eventHandlerBroker: null,
                configurationBroker: this.configurationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);

            var handlerNotFoundEventCallV2Exception =
                new HandlerNotFoundEventCallV2Exception(
                    message: "No event call handler was found, fix the errors and try again.");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: handlerNotFoundEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                serviceWithNullBroker.RunEventCallV2Async(someEventCallV2, randomCancellationToken);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task ShouldThrowValidationExceptionOnRunIfHandlerRegistrationCountIsInvalidAndLogItAsync(
            int matchingHandlerCount)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomHandlerId = Guid.NewGuid();
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            someEventCallV2.HandlerId = randomHandlerId;

            IEventHandler[] handlers;

            if (matchingHandlerCount == 0)
            {
                var noMatchHandlerMock = new Mock<IEventHandler>();
                noMatchHandlerMock.SetupGet(h => h.Id).Returns(Guid.NewGuid());
                handlers = new[] { noMatchHandlerMock.Object };
            }
            else
            {
                var matchHandlerMock1 = new Mock<IEventHandler>();
                matchHandlerMock1.SetupGet(h => h.Id).Returns(randomHandlerId);

                var matchHandlerMock2 = new Mock<IEventHandler>();
                matchHandlerMock2.SetupGet(h => h.Id).Returns(randomHandlerId);

                handlers = new[] { matchHandlerMock1.Object, matchHandlerMock2.Object };
            }

            var localBrokerMock = new Mock<IEventHandlerBroker>();
            localBrokerMock.Setup(b => b.GetAll()).Returns(handlers);

            IEventCallV2Service localService = new EventCallV2Service(
                eventHandlerBroker: localBrokerMock.Object,
                configurationBroker: this.configurationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);

            var invalidEventCallV2Exception =
                new InvalidEventCallV2Exception(
                    message: "EventHandlerBrokers on event call is invalid, fix the errors and try again.");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.HandlerId),
                values: matchingHandlerCount == 0
                    ? $"No handler found that matches '{randomHandlerId}', " +
                        $"fix registrations and try again."
                    : $"Multiple providers found that matches '{randomHandlerId}', " +
                        $"fix registrations and try again.");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                localService.RunEventCallV2Async(someEventCallV2, randomCancellationToken);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                Times.Once);

            localBrokerMock.Verify(b => b.GetAll(), Times.AtLeastOnce);
            localBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
