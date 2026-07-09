// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.EventParticipants.V2
{
    internal partial class EventParticipantV2ProcessingService
    {
        private static void ValidateOnRetrieveOrAddEventParticipantV2(EventParticipantV2 eventParticipantV2)
        {
            ValidateEventParticipantV2IsNotNull(eventParticipantV2);

            Validate(
                message: "Event participant is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventParticipantV2.Id),
                Parameter: nameof(EventParticipantV2.Id)));
        }

        private static void ValidateEventParticipantV2Id(Guid eventParticipantV2Id)
        {
            Validate(
                message: "Event participant is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventParticipantV2Id),
                Parameter: nameof(EventParticipantV2.Id)));
        }

        private static void ValidateEventParticipantV2IsNotNull(EventParticipantV2 eventParticipantV2)
        {
            if (eventParticipantV2 is null)
            {
                throw new NullEventParticipantV2ProcessingException(
                    message: "Event participant is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventParticipantV2ProcessingException =
                new InvalidEventParticipantV2ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventParticipantV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventParticipantV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
