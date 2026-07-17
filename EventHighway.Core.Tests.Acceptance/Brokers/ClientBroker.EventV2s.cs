// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        // Event submission requires a participant id; tests that don't exercise participants
        // fall back to a shared, fixed-id default participant so they stay focused on their
        // own concern.
        private static readonly Guid DefaultEventParticipantV2Id =
            Guid.Parse("9c1f6e5a-0d2b-4b7e-8a3f-5e4c1b2d3a90");

        private bool defaultEventParticipantV2Ensured;

        public async ValueTask<EventV2> SubmitEventV2Async(EventV2 eventV2)
        {
            if (eventV2.EventParticipantV2Id == Guid.Empty)
            {
                eventV2.EventParticipantV2Id =
                    await EnsureDefaultEventParticipantV2Async();
            }

            return await this.eventHighwayClient.V2.EventV2Client.SubmitEventV2Async(eventV2);
        }

        public async ValueTask<EventV2> SubmitEventV2WithoutDefaultsAsync(EventV2 eventV2) =>
            await this.eventHighwayClient.V2.EventV2Client.SubmitEventV2Async(eventV2);

        private async ValueTask<Guid> EnsureDefaultEventParticipantV2Async()
        {
            if (this.defaultEventParticipantV2Ensured)
            {
                return DefaultEventParticipantV2Id;
            }

            DateTimeOffset now = DateTimeOffset.UtcNow;

            await RetrieveOrAddEventParticipantV2Async(new EventParticipantV2
            {
                Id = DefaultEventParticipantV2Id,
                Name = "Acceptance Default Participant",
                Description = "Shared default participant for acceptance event submissions.",
                ContactEmail = "acceptance@eventhighway.local",
                ContactPhone = "0000000000",
                IsActive = true,
                CreatedDate = now,
                UpdatedDate = now
            });

            this.defaultEventParticipantV2Ensured = true;

            return DefaultEventParticipantV2Id;
        }

        public async ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sAsync() =>
            await this.eventHighwayClient.V2.EventV2Client.RetrieveAllEventV2sAsync();

        public async ValueTask<EventV2> RetrieveEventV2ByIdAsync(Guid eventV2Id) =>
            await this.eventHighwayClient.V2.EventV2Client.RetrieveEventV2ByIdAsync(eventV2Id);

        public async ValueTask FireScheduledPendingEventV2sAsync() =>
            await this.eventHighwayClient.V2.EventV2Client.FireScheduledPendingEventV2sAsync();

        public async ValueTask<EventV2> RemoveEventV2ByIdAsync(Guid eventV2Id) =>
            await this.eventHighwayClient.V2.EventV2Client.RemoveEventV2ByIdAsync(eventV2Id);
    }
}
