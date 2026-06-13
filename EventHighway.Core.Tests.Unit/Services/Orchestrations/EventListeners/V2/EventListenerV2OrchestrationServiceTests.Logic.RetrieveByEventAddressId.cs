// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV2sByEventAddressIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventAddressId = GetRandomId();
            Guid inputEventAddressId = randomEventAddressId;

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s();

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                randomEventListenerV2s;

            IQueryable<EventListenerV2> expectedEventListenerV2s =
                retrievedEventListenerV2s.DeepClone();

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputEventAddressId,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventListenerV2s);

            // when
            IQueryable<EventListenerV2> actualEventListenerV2s =
                await this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputEventAddressId,
                        randomCancellationToken);

            // then
            actualEventListenerV2s.Should()
                .BeEquivalentTo(expectedEventListenerV2s);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputEventAddressId,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
