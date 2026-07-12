// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldRegisterEventHandlerV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;
            IEventHandler expectedEventHandler = inputEventHandler;

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2Service.RegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(inputEventHandler),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
