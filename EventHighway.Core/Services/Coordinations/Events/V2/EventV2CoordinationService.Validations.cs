// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Coordinations.Events.V2
{
    internal partial class EventV2CoordinationService
    {
        private static void ValidateEventV2Id(Guid eventV2Id)
        {
            Validate(
                (Rule: IsInvalid(eventV2Id),
                Parameter: nameof(EventV2.Id)));
        }

        private static void ValidateEventV2Query(EventV2Query eventV2Query)
        {
            ValidateEventV2QueryIsNotNull(eventV2Query);

            ValidateQuery(
                (Rule: IsNegative(eventV2Query.Skip),
                Parameter: nameof(EventV2Query.Skip)),

                (Rule: IsOutOfRange(eventV2Query.Take),
                Parameter: nameof(EventV2Query.Take)),

                (Rule: IsBefore(
                    firstDate: eventV2Query.CreatedTo,
                    secondDate: eventV2Query.CreatedFrom,
                    secondDateName: nameof(EventV2Query.CreatedFrom)),
                Parameter: nameof(EventV2Query.CreatedTo)),

                (Rule: IsBefore(
                    firstDate: eventV2Query.ScheduledTo,
                    secondDate: eventV2Query.ScheduledFrom,
                    secondDateName: nameof(EventV2Query.ScheduledFrom)),
                Parameter: nameof(EventV2Query.ScheduledTo)));
        }

        private static void ValidateEventV2QueryIsNotNull(EventV2Query eventV2Query)
        {
            if (eventV2Query is null)
            {
                throw new NullEventV2QueryCoordinationException(
                    message: "Event query is null.");
            }
        }

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

        private static void ValidateQuery(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV2QueryCoordinationException =
                new InvalidEventV2QueryCoordinationException(
                    message: "Event query is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV2QueryCoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV2QueryCoordinationException.ThrowIfContainsErrors();
        }

        private static void ValidateEventV2IsNotNull(EventV2 eventV2)
        {
            if (eventV2 is null)
            {
                throw new NullEventV2CoordinationException(
                    message: "Event is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV2CoordinationException =
                new InvalidEventV2CoordinationException(
                    message: "Event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV2CoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV2CoordinationException.ThrowIfContainsErrors();
        }
    }
}
