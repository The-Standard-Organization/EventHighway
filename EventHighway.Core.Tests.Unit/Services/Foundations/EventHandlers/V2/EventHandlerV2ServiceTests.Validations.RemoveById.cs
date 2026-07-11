// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
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
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    invalidEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ValidationException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Remove(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
