// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV2sByEventAddressIdByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventAddressId = GetRandomId();
            Guid targetHandlerId = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<EventListenerV2> matchingEventListenerV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventListenerV2 matchingEventListenerV2 = CreateRandomEventListenerV2();
                    matchingEventListenerV2.HandlerId = targetHandlerId;
                    matchingEventListenerV2.CreatedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingEventListenerV2;
                }).ToList();

            EventListenerV2 differentHandlerEventListenerV2 = CreateRandomEventListenerV2();

            IQueryable<EventListenerV2> retrievedEventListenerV2s = matchingEventListenerV2s
                .Append(differentHandlerEventListenerV2)
                .AsQueryable();

            var inputEventListenerV2Query = new EventListenerV2Query
            {
                HandlerId = targetHandlerId,
                Skip = 1,
                Take = 2
            };

            List<EventListenerV2> expectedEventListenerV2s = matchingEventListenerV2s
                .OrderByDescending(eventListenerV2 => eventListenerV2.CreatedDate)
                .ThenBy(eventListenerV2 => eventListenerV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputEventAddressId,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventListenerV2s);

            // when
            IReadOnlyList<EventListenerV2> actualEventListenerV2s =
                await this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                        inputEventAddressId,
                        inputEventListenerV2Query,
                        randomCancellationToken);

            // then
            actualEventListenerV2s.Should().BeEquivalentTo(
                expectedEventListenerV2s, options => options.WithStrictOrdering());

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputEventAddressId,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
