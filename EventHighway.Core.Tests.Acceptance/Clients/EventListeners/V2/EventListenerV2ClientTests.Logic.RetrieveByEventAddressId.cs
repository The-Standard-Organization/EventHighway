// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V2
{
    public partial class EventListenerV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV2sByEventAddressIdAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id =
                randomEventAddressV2.Id;

            EventParticipantV2 randomEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2(
                    inputEventAddressV2Id,
                    randomEventParticipantV2.Id);

            await this.clientBroker.RegisterEventListenerV2Async(
                randomEventListenerV2);

            IReadOnlyList<EventListenerV2> inputEventListenerV2s =
                new[] { randomEventListenerV2 };

            IReadOnlyList<EventListenerV2> expectedEventListenerV2s =
                inputEventListenerV2s.DeepClone();

            // when
            IReadOnlyList<EventListenerV2> actualEventListenerV2s =
                await this.clientBroker
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputEventAddressV2Id);

            // then
            actualEventListenerV2s.Should()
                .BeEquivalentTo(expectedEventListenerV2s);

            await this.clientBroker
                .RemoveEventListenerV2ByIdAsync(
                    randomEventListenerV2.Id);

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(
                    inputEventAddressV2Id);
        }
    }
}
