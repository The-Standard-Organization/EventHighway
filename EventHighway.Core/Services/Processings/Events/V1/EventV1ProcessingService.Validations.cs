// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.Events.V1
{
    internal partial class EventV1ProcessingService
    {
        private static void ValidateEventV1IsNotNull(EventV1 listenerEventV1)
        {
            if (listenerEventV1 is null)
            {
                throw new NullEventV1ProcessingException(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventV1Id(Guid eventV1Id)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventV1Id),
                Parameter: nameof(EventV1.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV1ProcessingException =
                new InvalidEventV1ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV1ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV1ProcessingException.ThrowIfContainsErrors();
        }
    }
}
