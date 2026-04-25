// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal partial class EventV1CoordinationService
    {
        private static void ValidateEventId(Guid eventId)
        {
            Validate(
                (Rule: IsInvalid(eventId),
                Parameter: nameof(EventV1.Id)));
        }

        private static void ValidateEventIsNotNull(EventV1 @event)
        {
            if (@event is null)
            {
                throw new NullEventV1CoordinationException(
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
            var invalidEventV1CoordinationException =
                new InvalidEventV1CoordinationException(
                    message: "Event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV1CoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV1CoordinationException.ThrowIfContainsErrors();
        }
    }
}
