// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventParticipantV2sAsync()
        {
            // given
            IEnumerable<EventParticipantV2> randomEventParticipantV2s =
                await CreateRandomEventParticipantV2sAsync();

            IEnumerable<EventParticipantV2> inputEventParticipantV2s =
                randomEventParticipantV2s;

            IEnumerable<EventParticipantV2> expectedEventParticipantV2s =
                inputEventParticipantV2s.DeepClone();

            // when
            IReadOnlyList<EventParticipantV2> actualEventParticipantV2s =
                await this.clientBroker
                    .RetrieveAllEventParticipantV2sAsync(new EventParticipantV2Query
                    {
                        Take = 1000
                    });

            // then
            HashSet<Guid> expectedIds =
                expectedEventParticipantV2s.Select(e => e.Id).ToHashSet();

            actualEventParticipantV2s
                .Where(ep => expectedIds.Contains(ep.Id))
                .Should().BeEquivalentTo(expectedEventParticipantV2s);

            foreach (EventParticipantV2 randomEventParticipantV2
                in randomEventParticipantV2s)
            {
                await this.clientBroker
                    .RemoveEventParticipantV2ByIdAsync(
                        randomEventParticipantV2.Id);
            }
        }
    }
}
