// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
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
        public async Task ShouldRetrieveAllRegisteredAndStorageEventHandlerV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<IEventHandler> randomEventHandlers = CreateRandomEventHandlers();
            IQueryable<IEventHandler> registeredEventHandlers = randomEventHandlers;

            List<EventHandlerV2> registeredEventHandlerV2s =
                registeredEventHandlers.Select(eventHandler => new EventHandlerV2
                {
                    Id = eventHandler.Id,
                    Name = eventHandler.Name
                }).ToList();

            List<EventHandlerV2> storageOnlyEventHandlerV2s =
                CreateRandomEventHandlerV2s().ToList();

            var overlappingStorageEventHandlerV2 = new EventHandlerV2
            {
                Id = registeredEventHandlerV2s.First().Id,
                Name = GetRandomString()
            };

            IQueryable<EventHandlerV2> storageEventHandlerV2s =
                storageOnlyEventHandlerV2s.Append(overlappingStorageEventHandlerV2)
                    .AsQueryable();

            List<EventHandlerV2> expectedEventHandlerV2s =
                registeredEventHandlerV2s.Concat(storageOnlyEventHandlerV2s).ToList();

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(registeredEventHandlers);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken))
                    .ReturnsAsync(storageEventHandlerV2s);

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
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveAllEventHandlerV2sFromStorageWhenNoneRegisteredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<IEventHandler> emptyEventHandlers =
                Enumerable.Empty<IEventHandler>().AsQueryable();

            IQueryable<EventHandlerV2> randomEventHandlerV2s = CreateRandomEventHandlerV2s();
            IQueryable<EventHandlerV2> storageEventHandlerV2s = randomEventHandlerV2s;
            IQueryable<EventHandlerV2> expectedEventHandlerV2s = storageEventHandlerV2s;

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEventHandlers);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken))
                    .ReturnsAsync(storageEventHandlerV2s);

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
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
