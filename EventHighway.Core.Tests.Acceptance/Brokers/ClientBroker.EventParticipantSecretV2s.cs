// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2) =>
                await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                    .AddEventParticipantSecretV2Async(eventParticipantSecretV2);

        public async ValueTask<IReadOnlyList<EventParticipantSecretV2>>
            RetrieveAllEventParticipantSecretV2sAsync(
                EventParticipantSecretV2Query eventParticipantSecretV2Query) =>
                await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                    .RetrieveAllEventParticipantSecretV2sAsync(eventParticipantSecretV2Query);

        public async ValueTask<EventParticipantSecretV2> RetrieveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id) =>
                await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                    .RetrieveEventParticipantSecretV2ByIdAsync(eventParticipantSecretV2Id);

        public async ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2) =>
                await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                    .ModifyEventParticipantSecretV2Async(eventParticipantSecretV2);

        public async ValueTask<EventParticipantSecretV2> RemoveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id) =>
                await this.eventHighwayClient.V2.EventParticipantSecretV2Client
                    .RemoveEventParticipantSecretV2ByIdAsync(eventParticipantSecretV2Id);
    }
}
