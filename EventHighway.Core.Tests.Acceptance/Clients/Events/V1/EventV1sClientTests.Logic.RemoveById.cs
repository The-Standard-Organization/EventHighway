// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventV1ByIdAsync()
        {
            // given
            EventAddressV1 randomEventAddressV1 =
                await CreateRandomEventAddressV1Async();

            EventAddressV1 inputEventAddressV1 =
                randomEventAddressV1;

            Guid inputEventAddressV1Id =
                inputEventAddressV1.Id;

            int randomSeconds = GetRandomNumber();

            DateTimeOffset scheduledDate =
                TruncateToMicroseconds(DateTimeOffset.UtcNow
                    .AddSeconds(randomSeconds));

            EventV1 randomEventV1 =
                await SubmitEventV1Async(
                    inputEventAddressV1Id,
                    scheduledDate);

            Guid inputEventV1Id = randomEventV1.Id;
            EventV1 expectedEventV1 = randomEventV1;

            // when 
            EventV1 actualEventV1 =
                await this.clientBroker
                    .RemoveEventV1ByIdAsync(
                        inputEventV1Id);

            // then
            actualEventV1.Should()
                .BeEquivalentTo(expectedEventV1);

            await this.clientBroker
                .RemoveEventAddressV1ByIdAsync(
                    inputEventAddressV1Id);
        }
    }
}
