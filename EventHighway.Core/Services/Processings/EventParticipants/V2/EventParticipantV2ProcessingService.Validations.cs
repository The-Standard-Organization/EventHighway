// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
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

        private static void ValidateEventParticipantV2Query(
            EventParticipantV2Query eventParticipantV2Query)
        {
            ValidateEventParticipantV2QueryIsNotNull(eventParticipantV2Query);

            ValidateQuery(
                (Rule: IsNegative(eventParticipantV2Query.Skip),
                Parameter: nameof(EventParticipantV2Query.Skip)),

                (Rule: IsOutOfRange(eventParticipantV2Query.Take),
                Parameter: nameof(EventParticipantV2Query.Take)),

                (Rule: IsBefore(
                    firstDate: eventParticipantV2Query.CreatedTo,
                    secondDate: eventParticipantV2Query.CreatedFrom,
                    secondDateName: nameof(EventParticipantV2Query.CreatedFrom)),
                Parameter: nameof(EventParticipantV2Query.CreatedTo)));
        }

        private static void ValidateEventParticipantV2QueryIsNotNull(
            EventParticipantV2Query eventParticipantV2Query)
        {
            if (eventParticipantV2Query is null)
            {
                throw new NullEventParticipantV2QueryProcessingException(
                    message: "Event participant query is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static dynamic IsNegative(int value) => new
        {
            Condition = value < 0,
            Message = "Value must be zero or greater"
        };

        private static dynamic IsOutOfRange(int value) => new
        {
            Condition = value < 1 || value > 1000,
            Message = "Value must be between 1 and 1000"
        };

        private static dynamic IsBefore(
            DateTimeOffset? firstDate,
            DateTimeOffset? secondDate,
            string secondDateName) => new
            {
                Condition = firstDate is not null
                    && secondDate is not null
                    && firstDate < secondDate,

                Message = $"Date must be after {secondDateName}"
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

        private static void ValidateQuery(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventParticipantV2QueryProcessingException =
                new InvalidEventParticipantV2QueryProcessingException(
                    message: "Event participant query is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventParticipantV2QueryProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventParticipantV2QueryProcessingException.ThrowIfContainsErrors();
        }
    }
}
