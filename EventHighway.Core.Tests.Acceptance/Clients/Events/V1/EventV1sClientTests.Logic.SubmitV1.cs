// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
        public async Task ShouldSubmitEventV1AsyncV1()
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
                DateTimeOffset.Now
                    .AddSeconds(randomSeconds);

            EventV1 randomEventV1 =
                CreateRandomEventV1(
                    inputEventAddressV1Id,
                    scheduledDate);

            EventV1 inputEventV1 = randomEventV1;
            EventV1 expectedEventV1 = inputEventV1;

            // when 
            EventV1 actualEventV1 =
                await this.clientBroker
                    .SubmitEventV1AsyncV1(inputEventV1);

            // then
            actualEventV1.Should()
                .BeEquivalentTo(expectedEventV1);

            await this.clientBroker
                .RemoveEventV1ByIdAsync(
                    actualEventV1.Id);

            await this.clientBroker
                .RemoveEventAddressV1ByIdAsync(
                    inputEventAddressV1Id);
        }
    }
}
