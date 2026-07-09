// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        // Idempotent on the participant's (stable) Id: RetrieveOrAdd reuses an existing row with
        // the same Id, so re-running the seed does not insert duplicate participants.
        public ValueTask<EventParticipantV2> AddParticipantAsync(
            EventParticipantV2 participant,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventParticipantV2Client
                .RetrieveOrAddEventParticipantV2Async(participant, cancellationToken);
    }
}
