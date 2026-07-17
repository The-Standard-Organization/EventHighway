// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventParticipants.V2
{
    internal partial class EventParticipantV2OrchestrationService
    {
        private static void ValidateParticipantIdIsPresent(EventV2 eventV2)
        {
            if (eventV2.EventParticipantV2Id == Guid.Empty)
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant id is required.");
            }
        }

        private static void ValidateParticipant(EventParticipantV2 participant, DateTimeOffset now)
        {
            if (participant is null)
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant not found.");
            }

            if (participant.IsActive is false)
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant is not active.");
            }

            if (IsOutsideActiveWindow(participant.ActiveFrom, participant.ActiveTo, now))
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant is outside its active window.");
            }
        }

        private static void ValidateSecretIsProvidedWhenRequired(
            EventParticipantV2 participant,
            EventV2 eventV2)
        {
            if (participant.IsSecretRequired
                && string.IsNullOrWhiteSpace(eventV2.EventParticipantV2Secret))
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant secret is required.");
            }
        }

        private static void ValidateSecret(EventParticipantSecretV2 secret, DateTimeOffset now)
        {
            if (secret is null)
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant secret not found.");
            }

            if (secret.IsActive is false)
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant secret is not active.");
            }

            if (IsOutsideActiveWindow(secret.ActiveFrom, secret.ActiveTo, now))
            {
                throw new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant secret is outside its active window.");
            }
        }

        private static bool IsOutsideActiveWindow(
            DateTimeOffset? activeFrom,
            DateTimeOffset? activeTo,
            DateTimeOffset now)
        {
            return (activeFrom != null && activeFrom > now)
                || (activeTo != null && activeTo < now);
        }
    }
}
