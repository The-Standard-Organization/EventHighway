// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V2
{
    internal partial class EventListenerV2OrchestrationService
    {
        private static void ValidateEventListenerV2Id(Guid eventListenerV2Id)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV2Id),
                Parameter: nameof(EventListenerV2.Id)));
        }

        private static void ValidateEventAddressId(Guid eventAddressId)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventAddressId),
                Parameter: nameof(EventListenerV2.EventAddressV2Id)));
        }

        private static void ValidateEventListenerV2IsNotNull(EventListenerV2 eventListenerV2)
        {
            if (eventListenerV2 is null)
            {
                throw new NullEventListenerV2OrchestrationException(
                    message: "Event listener is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventListenerV2OrchestrationException =
                new InvalidEventListenerV2OrchestrationException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerV2OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerV2OrchestrationException.ThrowIfContainsErrors();
        }

        private static void ValidateEventListenerV2Query(EventListenerV2Query eventListenerV2Query)
        {
            ValidateEventListenerV2QueryIsNotNull(eventListenerV2Query);

            ValidateQuery(
                (Rule: IsNegative(eventListenerV2Query.Skip),
                Parameter: nameof(EventListenerV2Query.Skip)),

                (Rule: IsOutOfRange(eventListenerV2Query.Take),
                Parameter: nameof(EventListenerV2Query.Take)),

                (Rule: IsBefore(
                    firstDate: eventListenerV2Query.CreatedTo,
                    secondDate: eventListenerV2Query.CreatedFrom,
                    secondDateName: nameof(EventListenerV2Query.CreatedFrom)),
                Parameter: nameof(EventListenerV2Query.CreatedTo)));
        }

        private static void ValidateEventListenerV2QueryIsNotNull(EventListenerV2Query eventListenerV2Query)
        {
            if (eventListenerV2Query is null)
            {
                throw new NullEventListenerV2QueryOrchestrationException(
                    message: "Event listener query is null.");
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
            var invalidEventListenerV2QueryOrchestrationException =
                new InvalidEventListenerV2QueryOrchestrationException(
                    message: "Event listener query is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerV2QueryOrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerV2QueryOrchestrationException.ThrowIfContainsErrors();
        }
    }
}
