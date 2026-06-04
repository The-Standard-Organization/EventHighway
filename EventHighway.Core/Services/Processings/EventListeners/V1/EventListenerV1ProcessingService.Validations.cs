// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventListeners.V1
{
    internal partial class EventListenerV1ProcessingService
    {
        private static void ValidateEventAddressId(Guid eventAddressId)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventAddressId),
                Parameter: nameof(EventListenerV1.EventAddressId)));
        }

        private static void ValidateEventListenerV1Id(Guid eventListenerV1Id)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV1Id),
                Parameter: nameof(EventListenerV1.Id)));
        }

        private static void ValidateEventListenerV1IsNotNull(EventListenerV1 eventListenerV1)
        {
            if (eventListenerV1 is null)
            {
                throw new NullEventListenerV1ProcessingException(
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
            var invalidEventListenerV1ProcessingException =
                new InvalidEventListenerV1ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerV1ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerV1ProcessingException.ThrowIfContainsErrors();
        }
    }
}
