// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRegisterEventHandlerV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;
            IEventHandler registeredEventHandler = inputEventHandler;
            IEventHandler expectedEventHandler = registeredEventHandler;

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.AddEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken))
                        .ReturnsAsync(registeredEventHandler);

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
