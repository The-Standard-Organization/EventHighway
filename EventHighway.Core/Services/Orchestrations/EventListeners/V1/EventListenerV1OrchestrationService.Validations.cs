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
        private static void ValidateEventListenerId(Guid eventListenerId)
        {
            Validate(
                (Rule: IsInvalid(eventListenerId),
                Parameter: nameof(EventListenerV1.Id)));
        }

        private static void ValidateEventAddressId(Guid eventAddressId)
        {
            Validate(
                (Rule: IsInvalid(eventAddressId),
                Parameter: nameof(EventListenerV1.EventAddressId)));
        }

        private static void ValidateListenerEventId(Guid listenerEventId)
        {
            Validate(
                (Rule: IsInvalid(listenerEventId),
                Parameter: nameof(ListenerEventV1.Id)));
        }

        private static void ValidateEventListenerIsNotNull(EventListenerV1 eventListener)
        {
            if (eventListener is null)
            {
                throw new NullEventListenerV1OrchestrationException(
                    message: "Event listener is null.");
            }
        }

        private static void ValidateListenerEventIsNotNull(ListenerEventV1 listenerEvent)
        {
            if (listenerEvent is null)
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

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventListenerV1OrchestrationException =
                new InvalidEventListenerV1OrchestrationException(
                    message: "Event listener is invalid, fix the errors and try again.");

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
