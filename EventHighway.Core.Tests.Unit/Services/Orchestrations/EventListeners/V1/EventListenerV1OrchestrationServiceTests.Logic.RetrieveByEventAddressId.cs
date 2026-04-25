// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV1sByEventAddressIdAsync()
        {
            // given
            Guid randomEventAddressId = GetRandomId();
            Guid inputEventAddressId = randomEventAddressId;

            IQueryable<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s();

            IQueryable<EventListenerV1> retrievedEventListenerV1s =
                randomEventListenerV1s;

            IQueryable<EventListenerV1> expectedEventListenerV1s =
                retrievedEventListenerV1s.DeepClone();

            this.eventListenerV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    inputEventAddressId))
                        .ReturnsAsync(retrievedEventListenerV1s);

            // when
            IQueryable<EventListenerV1> actualEventListenerV1s =
                await this.eventListenerV1OrchestrationService
                    .RetrieveEventListenersByEventAddressIdAsync(
                        inputEventAddressId);

            // then
            actualEventListenerV1s.Should()
                .BeEquivalentTo(expectedEventListenerV1s);

            this.eventListenerV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    inputEventAddressId),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
