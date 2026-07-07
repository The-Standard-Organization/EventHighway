// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventV2sAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id =
                randomEventAddressV2.Id;

            int randomSeconds = GetRandomNumber();

            DateTimeOffset scheduledDate =
                TruncateToMicroseconds(DateTimeOffset.UtcNow
                    .AddSeconds(randomSeconds));

            EventV2 randomEventV2 =
                await SubmitEventV2Async(
                    inputEventAddressV2Id,
                    scheduledDate);

            Guid inputEventV2Id = randomEventV2.Id;
            EventV2 expectedEventV2 = randomEventV2;

            // when
            List<EventV2> actualEventV2s =
                (await this.clientBroker
                    .RetrieveAllEventV2sAsync())
                        .ToList();

            // then
            actualEventV2s.Should()
                .ContainEquivalentOf(expectedEventV2);

            await this.clientBroker
                .RemoveEventV2ByIdAsync(inputEventV2Id);

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
        }
    }
}
