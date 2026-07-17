// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Clients.EventParticipantSecrets.V2
{
    public interface IEventParticipantSecretV2Client
    {
        ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventParticipantSecretV2>> RetrieveAllEventParticipantSecretV2sAsync(
            EventParticipantSecretV2Query eventParticipantSecretV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> RetrieveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> RemoveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default);
    }
}
