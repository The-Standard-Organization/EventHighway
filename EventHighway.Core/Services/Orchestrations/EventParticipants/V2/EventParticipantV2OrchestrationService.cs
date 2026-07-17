// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Hashings;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;

namespace EventHighway.Core.Services.Orchestrations.EventParticipants.V2
{
    internal partial class EventParticipantV2OrchestrationService : IEventParticipantV2OrchestrationService
    {
        private readonly IEventParticipantV2Service eventParticipantV2Service;
        private readonly IEventParticipantSecretV2Service eventParticipantSecretV2Service;
        private readonly IHashBroker hashBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantV2OrchestrationService(
            IEventParticipantV2Service eventParticipantV2Service,
            IEventParticipantSecretV2Service eventParticipantSecretV2Service,
            IHashBroker hashBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventParticipantV2Service = eventParticipantV2Service;
            this.eventParticipantSecretV2Service = eventParticipantSecretV2Service;
            this.hashBroker = hashBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ValidateEventParticipantsAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (eventV2.EventParticipantV2Id is null)
            {
                ValidateParticipantSecretHasParticipantId(eventV2);

                return;
            }

            EventParticipantV2 maybeEventParticipantV2 =
                await this.eventParticipantV2Service.RetrieveEventParticipantV2ByIdAsync(
                    eventV2.EventParticipantV2Id.Value,
                    cancellationToken);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            ValidateParticipant(maybeEventParticipantV2, now);
            ValidateSecretIsProvidedWhenRequired(maybeEventParticipantV2, eventV2);

            if (string.IsNullOrWhiteSpace(eventV2.EventParticipantV2Secret) is false)
            {
                string hashedSecret = this.hashBroker.GenerateSha256Hash(
                    eventV2.EventParticipantV2Secret);

                IQueryable<EventParticipantSecretV2> eventParticipantSecretV2s =
                    await this.eventParticipantSecretV2Service
                        .RetrieveAllEventParticipantSecretV2sAsync(cancellationToken);

                EventParticipantSecretV2 maybeEventParticipantSecretV2 =
                    eventParticipantSecretV2s.FirstOrDefault(secret =>
                        secret.EventParticipantV2Id == eventV2.EventParticipantV2Id
                            && secret.Secret == hashedSecret);

                ValidateSecret(maybeEventParticipantSecretV2, now);
            }
        });
    }
}
