// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantV2Client
                    .AddEventParticipantV2Async(eventParticipantV2, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventParticipantV2>>
            RetrieveAllEventParticipantV2sAsync(
                EventParticipantV2Query eventParticipantV2Query,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(
                        eventParticipantV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantV2Client
                    .RetrieveEventParticipantV2ByIdAsync(eventParticipantV2Id, cancellationToken),
                cancellationToken);

        public ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantV2Client
                    .ModifyEventParticipantV2Async(eventParticipantV2, cancellationToken),
                cancellationToken);

        public ValueTask<EventParticipantV2> RemoveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventParticipantV2Client
                    .RemoveEventParticipantV2ByIdAsync(eventParticipantV2Id, cancellationToken),
                cancellationToken);
    }
}
