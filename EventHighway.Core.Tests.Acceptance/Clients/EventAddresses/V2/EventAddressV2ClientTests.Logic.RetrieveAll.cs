// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV2sAsync()
        {
            // given
            IQueryable<EventAddressV2> randomEventAddressV2s =
                await CreateRandomEventAddressV2sAsync();

            IQueryable<EventAddressV2> inputEventAddressV2s =
                randomEventAddressV2s;

            IQueryable<EventAddressV2> expectedEventAddressV2s =
                inputEventAddressV2s.DeepClone();

            // when
            IReadOnlyList<EventAddressV2> actualEventAddressV2s =
                await this.clientBroker
                    .RetrieveAllEventAddressV2sAsync(new EventAddressV2Query
                    {
                        Take = 1000
                    });

            // then
            HashSet<Guid> expectedIds =
                expectedEventAddressV2s.Select(e => e.Id).ToHashSet();

            actualEventAddressV2s
                .Where(ea => expectedIds.Contains(ea.Id))
                .Should().BeEquivalentTo(expectedEventAddressV2s);

            foreach (EventAddressV2 randomEventAddressV2
                in randomEventAddressV2s)
            {
                await this.clientBroker
                    .RemoveEventAddressV2ByIdAsync(
                        randomEventAddressV2.Id);
            }
        }
    }
}
