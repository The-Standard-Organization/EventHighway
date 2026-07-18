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
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventParticipants
{
    public partial class EventParticipantsViewService : IEventParticipantsViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantsViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventParticipantView>> RetrieveAllParticipantsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            var eventParticipantV2Query = new EventParticipantV2Query { Take = 1000 };
            var participants = new List<EventParticipantV2>();

            while (true)
            {
                IReadOnlyList<EventParticipantV2> participantPage =
                    await this.eventHighwayBroker.RetrieveAllEventParticipantV2sAsync(
                        eventParticipantV2Query, cancellationToken);

                participants.AddRange(participantPage);

                if (participantPage.Count < eventParticipantV2Query.Take)
                {
                    break;
                }

                eventParticipantV2Query.Skip += eventParticipantV2Query.Take;
            }

            return participants.Select(AsView).ToList();
        });

        public ValueTask<EventParticipantView> RetrieveParticipantByIdAsync(
            System.Guid participantId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventParticipantV2 participant =
                await this.eventHighwayBroker.RetrieveEventParticipantV2ByIdAsync(
                    participantId, cancellationToken);

            return AsView(participant);
        });

        public ValueTask<EventParticipantView> AddParticipantAsync(
            EventParticipantView participant,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var participantToAdd = new EventParticipantV2
            {
                Name = participant.Name,
                Description = participant.Description,
                ContactEmail = participant.ContactEmail,
                ContactPhone = participant.ContactPhone,
                IsActive = participant.IsActive,
                IsSecretRequired = participant.IsSecretRequired,
                ActiveFrom = participant.ActiveFrom,
                ActiveTo = participant.ActiveTo,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventParticipantV2 addedParticipant =
                await this.eventHighwayBroker.AddEventParticipantV2Async(
                    participantToAdd, cancellationToken);

            return AsView(addedParticipant);
        });

        public ValueTask<EventParticipantView> ModifyParticipantAsync(
            EventParticipantView participant,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventParticipantV2 existingParticipant =
                await this.eventHighwayBroker.RetrieveEventParticipantV2ByIdAsync(
                    participant.Id, cancellationToken);

            existingParticipant.Name = participant.Name;
            existingParticipant.Description = participant.Description;
            existingParticipant.ContactEmail = participant.ContactEmail;
            existingParticipant.ContactPhone = participant.ContactPhone;
            existingParticipant.IsActive = participant.IsActive;
            existingParticipant.IsSecretRequired = participant.IsSecretRequired;
            existingParticipant.ActiveFrom = participant.ActiveFrom;
            existingParticipant.ActiveTo = participant.ActiveTo;

            existingParticipant.UpdatedDate =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            EventParticipantV2 modifiedParticipant =
                await this.eventHighwayBroker.ModifyEventParticipantV2Async(
                    existingParticipant, cancellationToken);

            return AsView(modifiedParticipant);
        });

        public ValueTask<EventParticipantView> RemoveParticipantByIdAsync(
            Guid participantId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventParticipantV2 removedParticipant =
                await this.eventHighwayBroker.RemoveEventParticipantV2ByIdAsync(
                    participantId, cancellationToken);

            return AsView(removedParticipant);
        });

        private static EventParticipantView AsView(EventParticipantV2 participant) =>
            new EventParticipantView
            {
                Id = participant.Id,
                Name = participant.Name,
                Description = participant.Description,
                ContactEmail = participant.ContactEmail,
                ContactPhone = participant.ContactPhone,
                IsActive = participant.IsActive,
                IsSecretRequired = participant.IsSecretRequired,
                ActiveFrom = participant.ActiveFrom,
                ActiveTo = participant.ActiveTo
            };
    }
}
