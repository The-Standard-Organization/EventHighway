// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventAddresses.V1
{
    internal partial class EventAddressV1ProcessingService
    {
        private static void ValidateEventAddressV1Id(Guid eventAddressV1Id)
        {
            Validate(
                message: "Event address is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventAddressV1Id),
                Parameter: nameof(EventAddressV1.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventAddressV1ProcessingException =
                new InvalidEventAddressV1ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventAddressV1ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventAddressV1ProcessingException.ThrowIfContainsErrors();
        }
    }
}
