// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfEventHandlerV2IsNullAndLogItAsync()
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
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    nullEventHandler, randomCancellationToken);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                await Assert.ThrowsAsync<EventHandlerV2ValidationException>(
                    addEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
