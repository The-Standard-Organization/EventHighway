// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventArchiveV2sAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id =
                randomEventAddressV2.Id;

            EventV2 randomDeadEventV2 =
                await SubmitDeadEventV2Async(inputEventAddressV2Id);

            Guid expectedEventArchiveV2Id = randomDeadEventV2.Id;

            await this.clientBroker.ArchiveDeadEventV2sAsync();

            // when
            IReadOnlyList<EventArchiveV2> actualEventArchiveV2s =
                await this.clientBroker
                    .RetrieveAllEventArchiveV2sAsync(
                        new EventArchiveV2Query { Take = 1000 });

            // then
            actualEventArchiveV2s.Should()
                .Contain(eventArchiveV2 =>
                    eventArchiveV2.Id == expectedEventArchiveV2Id);

            await this.clientBroker
                .PurgeEventArchiveV2sAsync(DateTimeOffset.UtcNow.AddSeconds(1));

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
        }
    }
}
