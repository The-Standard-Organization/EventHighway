// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Tests.Acceptance.Extensions;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldModifyEventParticipantV2Async()
        {
            // given
            EventParticipantV2 randomEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventParticipantV2 modifiedEventParticipantV2 =
                CreateRandomEventParticipantV2();

            modifiedEventParticipantV2.Id =
                randomEventParticipantV2.Id;

            modifiedEventParticipantV2.CreatedDate =
                randomEventParticipantV2.CreatedDate;

            modifiedEventParticipantV2.UpdatedDate =
                DateTimeOffset.UtcNow;

            EventParticipantV2 expectedEventParticipantV2 =
                modifiedEventParticipantV2.DeepClone();

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.clientBroker
                    .ModifyEventParticipantV2Async(
                        modifiedEventParticipantV2);

            // then
            actualEventParticipantV2.Should()
                .BeEquivalentTo(expectedEventParticipantV2, options =>
                    options.WithDateTimeOffsetTolerance());

            await this.clientBroker
                .RemoveEventParticipantV2ByIdAsync(
                    randomEventParticipantV2.Id);
        }
    }
}
