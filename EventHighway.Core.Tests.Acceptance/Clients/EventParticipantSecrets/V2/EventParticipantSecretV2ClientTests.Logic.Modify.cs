// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Tests.Acceptance.Extensions;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldModifyEventParticipantSecretV2Async()
        {
            // given
            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                await CreateRandomEventParticipantSecretV2Async();

            EventParticipantSecretV2 modifiedEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(
                    randomEventParticipantSecretV2.EventParticipantV2Id);

            modifiedEventParticipantSecretV2.Id =
                randomEventParticipantSecretV2.Id;

            modifiedEventParticipantSecretV2.Secret =
                randomEventParticipantSecretV2.Secret;

            modifiedEventParticipantSecretV2.CreatedDate =
                randomEventParticipantSecretV2.CreatedDate;

            modifiedEventParticipantSecretV2.UpdatedDate =
                DateTimeOffset.UtcNow;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                modifiedEventParticipantSecretV2.DeepClone();

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.clientBroker
                    .ModifyEventParticipantSecretV2Async(
                        modifiedEventParticipantSecretV2);

            // then
            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2, options =>
                    options.WithDateTimeOffsetTolerance());

            await this.clientBroker
                .RemoveEventParticipantSecretV2ByIdAsync(
                    randomEventParticipantSecretV2.Id);
        }
    }
}
