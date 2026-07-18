// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventHandlerV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string targetName = GetRandomString();

            IQueryable<IEventHandler> registeredEventHandlers = CreateRandomEventHandlers();

            List<EventHandlerV2> matchingEventHandlerV2s =
                Enumerable.Range(start: 0, count: 4).Select(_ =>
                    new EventHandlerV2
                    {
                        Id = GetRandomId(),
                        Name = targetName
                    }).ToList();

            var differentNameEventHandlerV2 = new EventHandlerV2
            {
                Id = GetRandomId(),
                Name = GetRandomString()
            };

            IQueryable<EventHandlerV2> storageEventHandlerV2s = matchingEventHandlerV2s
                .Append(differentNameEventHandlerV2)
                .AsQueryable();

            var inputEventHandlerV2Query = new EventHandlerV2Query
            {
                Name = targetName,
                Skip = 1,
                Take = 2
            };

            List<EventHandlerV2> expectedEventHandlerV2s = matchingEventHandlerV2s
                .OrderBy(eventHandlerV2 => eventHandlerV2.Name)
                .ThenBy(eventHandlerV2 => eventHandlerV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(registeredEventHandlers);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sFromStorageAsync(randomCancellationToken))
                    .ReturnsAsync(storageEventHandlerV2s);

            // when
            IReadOnlyList<EventHandlerV2> actualEventHandlerV2s =
                await this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(
                    inputEventHandlerV2Query, randomCancellationToken);

            // then
            actualEventHandlerV2s.Should().BeEquivalentTo(
                expectedEventHandlerV2s, options => options.WithStrictOrdering());

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
