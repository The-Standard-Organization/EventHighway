// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Clients.EventParticipants.V2
{
    public interface IEventParticipantV2Client
    {
        ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantV2> RetrieveOrAddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantV2> RemoveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default);
    }
}
