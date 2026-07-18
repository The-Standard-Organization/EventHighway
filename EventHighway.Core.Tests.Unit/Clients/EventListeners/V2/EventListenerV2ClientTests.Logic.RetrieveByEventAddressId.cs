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
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V2
{
    public partial class EventListenerV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV2sByEventAddressIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventAddressId = GetRandomId();
            Guid inputEventAddressId = randomEventAddressId;
            var inputEventListenerV2Query = new EventListenerV2Query();

            IReadOnlyList<EventListenerV2> retrievedEventListenerV2s =
                CreateRandomEventListenerV2s().ToList();

            IReadOnlyList<EventListenerV2> expectedEventListenerV2s =
                retrievedEventListenerV2s.DeepClone();

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    inputEventAddressId,
                    inputEventListenerV2Query,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventListenerV2s);

            // when
            IReadOnlyList<EventListenerV2> actualEventListenerV2s =
                await this.eventListenerV2Client
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputEventAddressId,
                        inputEventListenerV2Query,
                        randomCancellationToken);

            // then
            actualEventListenerV2s.Should()
                .BeEquivalentTo(expectedEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    inputEventAddressId,
                    inputEventListenerV2Query,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
