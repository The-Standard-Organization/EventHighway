// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationService
    {
        private static void ValidateEventCallIsNotNull(EventCallV1 eventCall)
        {
            if (eventCall is null)
            {
                throw new NullEventCallV1OrchestrationException(
                    message: "Event call is null.");
            }
        }

        private static void ValidateEventIsNotNull(EventV1 @event)
        {
            if (@event is null)
            {
                throw new NullEventV1OrchestrationException(
                    message: "Event is null.");
            }
        }

        private static void ValidateListenerEventExists(EventAddressV1 eventAddress, Guid eventAddressId)
        {
            if (eventAddress is null)
            {
                throw new NotFoundEventAddressV1OrchestrationException(
                    message: $"Could not find event address with id: {eventAddressId}.");
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
            var invalidEventV1OrchestrationException =
                new InvalidEventV1OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV1OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV1OrchestrationException.ThrowIfContainsErrors();
        }
    }
}
