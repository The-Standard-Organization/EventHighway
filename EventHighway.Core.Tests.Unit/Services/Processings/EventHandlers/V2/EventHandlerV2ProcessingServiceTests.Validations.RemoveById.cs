// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventHandlerV2Id = Guid.Empty;

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
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2ProcessingService.RemoveEventHandlerV2ByIdAsync(
                    invalidEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ProcessingValidationException actualEventHandlerV2ProcessingValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ProcessingValidationException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingValidationException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
