// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldRetrieveAllListenerEventV2sAsync()
        {
            // given
            IQueryable<ListenerEventV2> randomListenerEventV2s =
                await CreateRandomListenerEventV2sAsync();

            IQueryable<ListenerEventV2> inputListenerEventV2s =
                randomListenerEventV2s;

            IQueryable<ListenerEventV2> expectedListenerEventV2s =
                inputListenerEventV2s.DeepClone();

            // when
            List<ListenerEventV2> actualListenerEventV2s =
              (await this.clientBroker
                  .RetrieveAllListenerEventV2sAsync())
                      .ToList();

            // then
            actualListenerEventV2s.Should()
                .BeEquivalentTo(expectedListenerEventV2s);

            foreach (ListenerEventV2 actualListenerEventV2
                in actualListenerEventV2s)
            {
                await this.clientBroker
                    .RemoveListenerEventV2ByIdAsync(
                        actualListenerEventV2.Id);
            }
        }
    }
}
