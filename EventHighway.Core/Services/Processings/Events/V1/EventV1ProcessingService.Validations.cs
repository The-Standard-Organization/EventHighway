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
        private static void ValidateEventIsNotNull(EventV1 listenerEvent)
        {
            if (listenerEvent is null)
            {
                throw new NullEventV1ProcessingException(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventId(Guid eventId)
        {
            Validate(
                (Rule: IsInvalid(eventId),
                Parameter: nameof(EventV1.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV1ProcessingException =
                new InvalidEventV1ProcessingException(
                    message: "Event is invalid, fix the errors and try again.");

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
