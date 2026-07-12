// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventParticipantSecrets
{
    public partial class EventParticipantSecretsViewService : IEventParticipantSecretsViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantSecretsViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventParticipantSecretView>>
            RetrieveSecretsByParticipantAsync(
                Guid participantId,
                CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IEnumerable<EventParticipantSecretV2> secrets =
                await this.eventHighwayBroker.RetrieveAllEventParticipantSecretV2sAsync(
                    cancellationToken);

            return secrets
                .Where(secret => secret.EventParticipantV2Id == participantId)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventParticipantSecretView> AddSecretAsync(
            EventParticipantSecretView secret,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var secretToAdd = new EventParticipantSecretV2
            {
                Id = Guid.NewGuid(),
                Secret = secret.Secret,
                IsActive = secret.IsActive,
                ActiveFrom = secret.ActiveFrom,
                ActiveTo = secret.ActiveTo,
                EventParticipantV2Id = secret.EventParticipantV2Id,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventParticipantSecretV2 addedSecret =
                await this.eventHighwayBroker.AddEventParticipantSecretV2Async(
                    secretToAdd, cancellationToken);

            return AsView(addedSecret);
        });

        public ValueTask<EventParticipantSecretView> ModifySecretAsync(
            EventParticipantSecretView secret,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IEnumerable<EventParticipantSecretV2> secrets =
                await this.eventHighwayBroker.RetrieveAllEventParticipantSecretV2sAsync(
                    cancellationToken);

            EventParticipantSecretV2 existingSecret =
                secrets.First(retrievedSecret => retrievedSecret.Id == secret.Id);

            existingSecret.IsActive = secret.IsActive;
            existingSecret.ActiveFrom = secret.ActiveFrom;
            existingSecret.ActiveTo = secret.ActiveTo;

            existingSecret.UpdatedDate =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            EventParticipantSecretV2 modifiedSecret =
                await this.eventHighwayBroker.ModifyEventParticipantSecretV2Async(
                    existingSecret, cancellationToken);

            return AsView(modifiedSecret);
        });

        public ValueTask<EventParticipantSecretView> RemoveSecretByIdAsync(
            Guid secretId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventParticipantSecretV2 removedSecret =
                await this.eventHighwayBroker.RemoveEventParticipantSecretV2ByIdAsync(
                    secretId, cancellationToken);

            return AsView(removedSecret);
        });

        private static EventParticipantSecretView AsView(EventParticipantSecretV2 secret) =>
            new EventParticipantSecretView
            {
                Id = secret.Id,
                Secret = secret.Secret,
                IsActive = secret.IsActive,
                ActiveFrom = secret.ActiveFrom,
                ActiveTo = secret.ActiveTo,
                EventParticipantV2Id = secret.EventParticipantV2Id
            };
    }
}
