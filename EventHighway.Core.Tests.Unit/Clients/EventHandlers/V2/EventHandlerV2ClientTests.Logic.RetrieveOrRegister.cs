// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveOrRegisterEventHandlerV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;
            IEventHandler retrievedOrRegisteredEventHandler = inputEventHandler;
            IEventHandler expectedEventHandler = retrievedOrRegisteredEventHandler;

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveOrRegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken))
                        .ReturnsAsync(retrievedOrRegisteredEventHandler);

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2Client.RetrieveOrRegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveOrRegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
