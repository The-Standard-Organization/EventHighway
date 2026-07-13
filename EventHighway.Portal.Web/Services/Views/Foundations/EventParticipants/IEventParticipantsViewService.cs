// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventParticipants
{
    public interface IEventParticipantsViewService
    {
        ValueTask<List<EventParticipantView>> RetrieveAllParticipantsAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantView> RetrieveParticipantByIdAsync(
            Guid participantId,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantView> AddParticipantAsync(
            EventParticipantView participant,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantView> ModifyParticipantAsync(
            EventParticipantView participant,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantView> RemoveParticipantByIdAsync(
            Guid participantId,
            CancellationToken cancellationToken = default);
    }
}
