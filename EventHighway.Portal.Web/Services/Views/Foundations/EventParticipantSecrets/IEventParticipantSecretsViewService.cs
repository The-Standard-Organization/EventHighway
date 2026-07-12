// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventParticipantSecrets
{
    public interface IEventParticipantSecretsViewService
    {
        ValueTask<List<EventParticipantSecretView>> RetrieveSecretsByParticipantAsync(
            Guid participantId,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretView> AddSecretAsync(
            EventParticipantSecretView secret,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretView> ModifySecretAsync(
            EventParticipantSecretView secret,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretView> RemoveSecretByIdAsync(
            Guid secretId,
            CancellationToken cancellationToken = default);
    }
}
