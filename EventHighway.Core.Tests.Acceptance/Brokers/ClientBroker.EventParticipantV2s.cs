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
        public async ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2) =>
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .AddEventParticipantV2Async(eventParticipantV2);

        public async ValueTask<EventParticipantV2> RetrieveOrAddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2) =>
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .RetrieveOrAddEventParticipantV2Async(eventParticipantV2);

        public async ValueTask<IEnumerable<EventParticipantV2>>
            RetrieveAllEventParticipantV2sAsync() =>
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync();

        public async ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id) =>
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .RetrieveEventParticipantV2ByIdAsync(eventParticipantV2Id);

        public async ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2) =>
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .ModifyEventParticipantV2Async(eventParticipantV2);

        public async ValueTask<EventParticipantV2> RemoveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id) =>
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .RemoveEventParticipantV2ByIdAsync(eventParticipantV2Id);
    }
}
