// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V1
{
    public partial class EventListenerV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV1sByEventAddressIdAsync()
        {
            // given
            Guid randomEventAddressId = GetRandomId();
            Guid inputEventAddressId = randomEventAddressId;

            List<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s(randomEventAddressId)
                    .ToList();

            List<EventListenerV1> randomOtherEventListenerV1s =
                CreateRandomEventListenerV1s()
                    .ToList();

            IQueryable<EventListenerV1> retrievedEventListenerV1s =
                randomEventListenerV1s.Union(randomOtherEventListenerV1s)
                    .AsQueryable();

            IQueryable<EventListenerV1> expectedEventListenerV1s =
                randomEventListenerV1s.AsQueryable();

            this.eventListenerV1ServiceMock.Setup(service =>
                service.RetrieveAllEventListenersAsync())
                    .ReturnsAsync(retrievedEventListenerV1s);

            // when
            IQueryable<EventListenerV1> actualEventListenerV1s =
                await this.eventListenerV1ProcessingService
                    .RetrieveEventListenersByEventAddressIdAsync(
                        inputEventAddressId);

            // then
            actualEventListenerV1s.Should()
                .BeEquivalentTo(expectedEventListenerV1s);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RetrieveAllEventListenersAsync(),
                    Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
