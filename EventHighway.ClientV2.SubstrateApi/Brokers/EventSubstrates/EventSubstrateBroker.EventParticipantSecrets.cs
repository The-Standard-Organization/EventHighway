// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        // Idempotent on the secret's (stable) Id: an existing row with the same Id is reused so
        // re-running the seed does not insert duplicate participant secrets.
        public ValueTask<EventParticipantSecretV2> AddParticipantSecretAsync(
            EventParticipantSecretV2 participantSecret,
            CancellationToken cancellationToken = default) =>
            this.databaseGate.ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<EventParticipantSecretV2> existingSecrets =
                        await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                            .RetrieveAllEventParticipantSecretV2sAsync(
                                new EventParticipantSecretV2Query
                                {
                                    EventParticipantV2Id = participantSecret.EventParticipantV2Id,
                                    Take = 1000
                                },
                                cancellationToken);

                    EventParticipantSecretV2 maybeSecret =
                        existingSecrets.FirstOrDefault(
                            existing => existing.Id == participantSecret.Id);

                    return maybeSecret
                        ?? await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                            .AddEventParticipantSecretV2Async(participantSecret, cancellationToken);
                },
                cancellationToken);
    }
}
