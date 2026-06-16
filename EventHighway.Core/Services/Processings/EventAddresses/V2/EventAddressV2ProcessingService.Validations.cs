// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
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
