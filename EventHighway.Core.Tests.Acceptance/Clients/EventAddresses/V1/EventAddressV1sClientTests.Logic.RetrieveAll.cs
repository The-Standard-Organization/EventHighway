// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V1
{
    public partial class EventAddressV1sClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV1sAsync()
        {
            // given
            IQueryable<EventAddressV1> randomEventAddressV1s =
                await CreateRandomEventAddressV1sAsync();

            IQueryable<EventAddressV1> inputEventAddressV1s =
                randomEventAddressV1s;

            IQueryable<EventAddressV1> expectedEventAddressV1s =
                inputEventAddressV1s.DeepClone();

            // when 
            List<EventAddressV1> actualEventAddressV1s =
                (await this.clientBroker.RetrieveAllEventAddressV1sAsync())
                    .ToList();

            // then
            actualEventAddressV1s.Should()
                .Contain(expectedEventAddressV1 =>
                    expectedEventAddressV1s.Any(expected =>
                        expected.Id == expectedEventAddressV1.Id));

            foreach (EventAddressV1 expectedEventAddressV1
                in expectedEventAddressV1s)
            {
                await this.clientBroker
                    .RemoveEventAddressV1ByIdAsync(
                        expectedEventAddressV1.Id);
            }
        }
    }
}
