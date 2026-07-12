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
        public async Task ShouldRetrieveEventHandlerV2IfItAlreadyExistsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;

            IQueryable<IEventHandler> retrievedEventHandlers =
                new[] { inputEventHandler }.AsQueryable();

            IEventHandler expectedEventHandler = inputEventHandler;

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventHandlers);

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2ProcessingService
                    .RetrieveOrRegisterEventHandlerV2Async(
                        inputEventHandler, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRegisterEventHandlerV2InMemoryOnlyIfItExistsInStorageAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;
            IEventHandler registeredEventHandler = inputEventHandler;
            IEventHandler expectedEventHandler = registeredEventHandler;

            IQueryable<IEventHandler> emptyEventHandlers =
                Enumerable.Empty<IEventHandler>().AsQueryable();

            IQueryable<EventHandlerV2> storageEventHandlerV2s = new[]
            {
                new EventHandlerV2
                {
                    Id = inputEventHandler.Id,
                    Name = inputEventHandler.Name
                }
            }.AsQueryable();

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEventHandlers);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken))
                    .ReturnsAsync(storageEventHandlerV2s);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken))
                        .ReturnsAsync(registeredEventHandler);

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2ProcessingService
                    .RetrieveOrRegisterEventHandlerV2Async(
                        inputEventHandler, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRegisterEventHandlerV2IfItDoesNotExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;
            IEventHandler registeredEventHandler = inputEventHandler;
            IEventHandler expectedEventHandler = registeredEventHandler;

            IQueryable<IEventHandler> emptyEventHandlers =
                Enumerable.Empty<IEventHandler>().AsQueryable();

            IQueryable<EventHandlerV2> emptyStorageEventHandlerV2s =
                Enumerable.Empty<EventHandlerV2>().AsQueryable();

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEventHandlers);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken))
                    .ReturnsAsync(emptyStorageEventHandlerV2s);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.AddEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken))
                        .ReturnsAsync(registeredEventHandler);

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2ProcessingService
                    .RetrieveOrRegisterEventHandlerV2Async(
                        inputEventHandler, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    inputEventHandler, randomCancellationToken),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
