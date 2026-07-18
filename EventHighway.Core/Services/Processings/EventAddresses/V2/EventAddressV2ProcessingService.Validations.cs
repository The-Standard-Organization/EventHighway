// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.EventAddresses.V2
{
    internal partial class EventAddressV2ProcessingService
    {
        private static void ValidateOnRegisterEventAddressV2(EventAddressV2 eventAddressV2) =>
            ValidateEventAddressV2IsNotNull(eventAddressV2);

        private static void ValidateOnRemoveEventAddressV2ById(Guid eventAddressV2Id) =>
            ValidateEventAddressV2Id(eventAddressV2Id);

        private static void ValidateOnRetrieveOrRegisterEventAddressV2(EventAddressV2 eventAddressV2)
        {
            ValidateEventAddressV2IsNotNull(eventAddressV2);

            Validate(
                message: "Event address is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventAddressV2.Id),
                Parameter: nameof(EventAddressV2.Id)));
        }

        private static void ValidateEventAddressV2Query(
            EventAddressV2Query eventAddressV2Query)
        {
            ValidateEventAddressV2QueryIsNotNull(eventAddressV2Query);

            ValidateQuery(
                (Rule: IsNegative(eventAddressV2Query.Skip),
                Parameter: nameof(EventAddressV2Query.Skip)),

                (Rule: IsOutOfRange(eventAddressV2Query.Take),
                Parameter: nameof(EventAddressV2Query.Take)),

                (Rule: IsBefore(
                    firstDate: eventAddressV2Query.CreatedTo,
                    secondDate: eventAddressV2Query.CreatedFrom,
                    secondDateName: nameof(EventAddressV2Query.CreatedFrom)),
                Parameter: nameof(EventAddressV2Query.CreatedTo)));
        }

        private static void ValidateEventAddressV2QueryIsNotNull(
            EventAddressV2Query eventAddressV2Query)
        {
            if (eventAddressV2Query is null)
            {
                throw new NullEventAddressV2QueryProcessingException(
                    message: "Event address query is null.");
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
            var invalidEventAddressV2QueryProcessingException =
                new InvalidEventAddressV2QueryProcessingException(
                    message: "Event address query is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventAddressV2QueryProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventAddressV2QueryProcessingException.ThrowIfContainsErrors();
        }

        private static void ValidateEventAddressV2IsNotNull(EventAddressV2 eventAddressV2)
        {
            if (eventAddressV2 is null)
            {
                throw new NullEventAddressV2ProcessingException(
                    message: "Event address is null.");
            }
        }

        private static void ValidateEventAddressV2Id(Guid eventAddressV2Id)
        {
            Validate(
                message: "Event address is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventAddressV2Id),
                Parameter: nameof(EventAddressV2.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventAddressV2ProcessingException =
                new InvalidEventAddressV2ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventAddressV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventAddressV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
