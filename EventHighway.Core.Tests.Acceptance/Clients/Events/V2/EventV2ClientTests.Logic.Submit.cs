// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldSubmitEventV2Async()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            EventAddressV2 inputEventAddressV2 =
                randomEventAddressV2;

            Guid inputEventAddressV2Id =
                inputEventAddressV2.Id;

            int randomSeconds = GetRandomNumber();

            DateTimeOffset scheduledDate =
                TruncateToMicroseconds(DateTimeOffset.UtcNow
                    .AddSeconds(randomSeconds));

            EventV2 randomEventV2 =
                CreateRandomEventV2(
                    inputEventAddressV2Id,
                    scheduledDate);

            EventV2 inputEventV2 = randomEventV2;
            EventV2 expectedEventV2 = inputEventV2;

            // when
            EventV2 actualEventV2 =
                await this.clientBroker
                    .SubmitEventV2Async(inputEventV2);

            // then
            actualEventV2.Should()
                .BeEquivalentTo(expectedEventV2);

            await this.clientBroker
                .RemoveEventV2ByIdAsync(
                    actualEventV2.Id);

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(
                    inputEventAddressV2Id);
        }
    }
}
