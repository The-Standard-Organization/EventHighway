// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventParticipantSecretV2sAsync()
        {
            // given
            IEnumerable<EventParticipantSecretV2> randomEventParticipantSecretV2s =
                await CreateRandomEventParticipantSecretV2sAsync();

            IEnumerable<EventParticipantSecretV2> inputEventParticipantSecretV2s =
                randomEventParticipantSecretV2s;

            IEnumerable<EventParticipantSecretV2> expectedEventParticipantSecretV2s =
                inputEventParticipantSecretV2s.DeepClone();

            foreach (EventParticipantSecretV2 expectedEventParticipantSecretV2
                in expectedEventParticipantSecretV2s)
            {
                expectedEventParticipantSecretV2.Secret = null;
            }

            // when
            IReadOnlyList<EventParticipantSecretV2> actualEventParticipantSecretV2s =
                await this.clientBroker
                    .RetrieveAllEventParticipantSecretV2sAsync(
                        new EventParticipantSecretV2Query { Take = 1000 });

            // then
            HashSet<Guid> expectedIds =
                expectedEventParticipantSecretV2s.Select(s => s.Id).ToHashSet();

            actualEventParticipantSecretV2s
                .Where(s => expectedIds.Contains(s.Id))
                .Should().BeEquivalentTo(expectedEventParticipantSecretV2s);

            foreach (EventParticipantSecretV2 randomEventParticipantSecretV2
                in randomEventParticipantSecretV2s)
            {
                await this.clientBroker
                    .RemoveEventParticipantSecretV2ByIdAsync(
                        randomEventParticipantSecretV2.Id);
            }
        }
    }
}
