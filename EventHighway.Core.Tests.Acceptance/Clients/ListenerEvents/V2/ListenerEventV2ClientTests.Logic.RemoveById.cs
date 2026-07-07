// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldRemoveListenerEventV2ByIdAsync()
        {
            // given
            List<ListenerEventV2> randomListenerEventV2s =
                (await CreateRandomListenerEventV2sAsync())
                    .ToList();

            ListenerEventV2 randomListenerEventV2 =
                randomListenerEventV2s.First();

            ListenerEventV2 inputListenerEventV2 =
                randomListenerEventV2;

            ListenerEventV2 expectedListenerEventV2 =
                inputListenerEventV2.DeepClone();

            Guid inputListenerEventV2Id =
                inputListenerEventV2.Id;

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.clientBroker
                    .RemoveListenerEventV2ByIdAsync(
                        inputListenerEventV2Id);

            // then
            actualListenerEventV2.Should()
                .BeEquivalentTo(expectedListenerEventV2);

            foreach (ListenerEventV2 remainingListenerEventV2
                in randomListenerEventV2s.Where(le => le.Id != inputListenerEventV2Id))
            {
                await this.clientBroker
                    .RemoveListenerEventV2ByIdAsync(
                        remainingListenerEventV2.Id);
            }
        }
    }
}
