// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V1
{
    internal partial class EventListenerV1OrchestrationService
    {
        private static void ValidateEventListenerV1Id(Guid eventListenerV1Id)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV1Id),
                Parameter: nameof(EventListenerV1.Id)));
        }

        private static void ValidateEventAddressId(Guid eventAddressId)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventAddressId),
                Parameter: nameof(EventListenerV1.EventAddressId)));
        }

        private static void ValidateListenerEventV1Id(Guid listenerEventV1Id)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(listenerEventV1Id),
                Parameter: nameof(ListenerEventV1.Id)));
        }

        private static void ValidateEventListenerV1IsNotNull(EventListenerV1 eventListenerV1)
        {
            if (eventListenerV1 is null)
            {
                throw new NullEventListenerV1OrchestrationException(
                    message: "Event listener is null.");
            }
        }

        private static void ValidateListenerEventV1IsNotNull(ListenerEventV1 listenerEventV1)
        {
            if (listenerEventV1 is null)
            {
                throw new NullListenerEventV1OrchestrationException(
                    message: "Listener event is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventListenerV1OrchestrationException =
                new InvalidEventListenerV1OrchestrationException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerV1OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerV1OrchestrationException.ThrowIfContainsErrors();
        }
    }
}
