// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;

namespace EventHighway.Core.Services.Processings.EventParticipants.V2
{
    internal interface IEventParticipantV2ProcessingService
    {
        ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantV2> RetrieveOrAddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventParticipantV2>> RetrieveEventParticipantV2sByQueryAsync(
            EventParticipantV2Query eventParticipantV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
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
