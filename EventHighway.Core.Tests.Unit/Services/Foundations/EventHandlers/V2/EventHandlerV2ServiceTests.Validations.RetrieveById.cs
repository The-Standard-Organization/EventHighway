// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventHandlerV2Id = Guid.Empty;

            var invalidEventHandlerV2Exception =
                new InvalidEventHandlerV2Exception(
                    message: "Event handler is invalid, fix the errors and try again.");

            invalidEventHandlerV2Exception.AddData(
                key: nameof(IEventHandler.Id),
                values: "Id required");

            var expectedEventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: invalidEventHandlerV2Exception);

            // when
            ValueTask<IEventHandler> retrieveEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RetrieveEventHandlerV2ByIdAsync(
                    invalidEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ValidationException>(
                    retrieveEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ValidationException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfEventHandlerV2IsNotFoundAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

            IEnumerable<IEventHandler> randomEventHandlers =
                CreateRandomEventHandlers();

            var notFoundEventHandlerV2Exception =
                new NotFoundEventHandlerV2Exception(
                    message: $"Could not find event handler with id: {someEventHandlerV2Id}.");

            var expectedEventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventHandlerV2Exception);

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.GetAll())
                    .Returns(randomEventHandlers);

            // when
            ValueTask<IEventHandler> retrieveEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RetrieveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ValidationException>(
                    retrieveEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ValidationException))),
                            Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
