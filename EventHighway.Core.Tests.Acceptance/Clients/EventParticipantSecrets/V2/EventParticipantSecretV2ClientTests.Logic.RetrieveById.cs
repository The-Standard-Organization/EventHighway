// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveEventParticipantSecretV2ByIdAsync()
        {
            // given
            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                await CreateRandomEventParticipantSecretV2Async();

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                randomEventParticipantSecretV2.DeepClone();

            expectedEventParticipantSecretV2.Secret = null;

            Guid inputEventParticipantSecretV2Id =
                randomEventParticipantSecretV2.Id;

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.clientBroker
                    .RetrieveEventParticipantSecretV2ByIdAsync(
                        inputEventParticipantSecretV2Id);

            // then
            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2);

            await this.clientBroker
                .RemoveEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id);
        }
    }
}
