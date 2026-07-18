// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldAddEventParticipantSecretV2Async()
        {
            // given
            EventParticipantV2 randomEventParticipantV2 =
                await CreateAndPersistRandomEventParticipantV2Async();

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomEventParticipantV2.Id);

            EventParticipantSecretV2 inputEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            string plainTextSecret = inputEventParticipantSecretV2.Secret;

            string expectedHashedSecret = Convert.ToHexStringLower(
                SHA256.HashData(Encoding.UTF8.GetBytes(plainTextSecret)));

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.clientBroker
                    .AddEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2);

            // then
            actualEventParticipantSecretV2.Secret.Should()
                .Be(expectedHashedSecret);

            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(inputEventParticipantSecretV2);

            await this.clientBroker
                .RemoveEventParticipantSecretV2ByIdAsync(
                    actualEventParticipantSecretV2.Id);

            await this.clientBroker
                .RemoveEventParticipantV2ByIdAsync(
                    randomEventParticipantV2.Id);
        }
    }
}
