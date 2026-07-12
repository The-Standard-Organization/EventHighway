// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnRegisterIfEventHandlerV2IsNullAndLogItAsync()
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
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    nullEventHandler, randomCancellationToken);

            EventHandlerV2ProcessingValidationException actualEventHandlerV2ProcessingValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ProcessingValidationException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingValidationException))),
                        Times.Once);

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
