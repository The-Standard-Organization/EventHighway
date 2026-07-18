// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantSecretV2Client
                    .AddEventParticipantSecretV2Async(eventParticipantSecretV2, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventParticipantSecretV2>>
            RetrieveAllEventParticipantSecretV2sAsync(
                EventParticipantSecretV2Query eventParticipantSecretV2Query,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantSecretV2Client
                    .RetrieveAllEventParticipantSecretV2sAsync(
                        eventParticipantSecretV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantSecretV2Client
                    .ModifyEventParticipantSecretV2Async(eventParticipantSecretV2, cancellationToken),
                cancellationToken);

        public ValueTask<EventParticipantSecretV2> RemoveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantSecretV2Client
                    .RemoveEventParticipantSecretV2ByIdAsync(eventParticipantSecretV2Id, cancellationToken),
                cancellationToken);
    }
}
