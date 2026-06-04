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
        private static void ValidateEventCallV1IsNotNull(EventCallV1 eventCallV1)
        {
            if (eventCallV1 is null)
            {
                throw new NullEventCallV1OrchestrationException(
                    message: "Event call is null.");
            }
        }

        private static void ValidateEventV1IsNotNull(EventV1 eventV1)
        {
            if (eventV1 is null)
            {
                throw new NullEventV1OrchestrationException(
                    message: "Event is null.");
            }
        }

        private static void ValidateListenerEventV1Exists(EventAddressV1 eventAddressV1, Guid eventAddressV1Id)
        {
            if (eventAddressV1 is null)
            {
                throw new NotFoundEventAddressV1OrchestrationException(
                    message: $"Could not find event address with id: {eventAddressV1Id}.");
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
            var invalidEventV1OrchestrationException = new InvalidEventV1OrchestrationException(message);

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
