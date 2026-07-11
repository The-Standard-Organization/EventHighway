// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllRegisteredEventHandlerV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<IEventHandler> randomEventHandlers = CreateRandomEventHandlers();
            IQueryable<IEventHandler> registeredEventHandlers = randomEventHandlers;

            IQueryable<EventHandlerV2> expectedEventHandlerV2s =
                registeredEventHandlers.Select(eventHandler => new EventHandlerV2
                {
                    Id = eventHandler.Id,
                    Name = eventHandler.Name
                });

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(registeredEventHandlers);

            // when
            IQueryable<EventHandlerV2> actualEventHandlerV2s =
                await this.eventHandlerV2ProcessingService.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            // then
            actualEventHandlerV2s.Should().BeEquivalentTo(expectedEventHandlerV2s);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
