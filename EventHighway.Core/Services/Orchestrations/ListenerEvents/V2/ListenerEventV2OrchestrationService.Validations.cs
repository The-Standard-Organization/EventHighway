// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.ListenerEvents.V2
{
    internal partial class ListenerEventV2OrchestrationService
    {
        private static void ValidateOnRetrieveBatchOfListenerEventV2sByEventIds(
            IEnumerable<Guid> eventV2Ids,
            int take)
        {
            Validate(
                message: "Listener event is invalid, fix the errors and try again.",

                (Rule: IsNull(eventV2Ids),
                Parameter: nameof(eventV2Ids)),

                (Rule: IsInvalid(take),
                Parameter: nameof(take)));
        }

        private static void ValidateOnBulkRemoveListenerEventV2s(
            IEnumerable<ListenerEventV2> listenerEventV2s)
        {
            Validate(
                message: "Listener event is invalid, fix the errors and try again.",

                (Rule: IsNull(listenerEventV2s),
                Parameter: nameof(listenerEventV2s)));
        }

        private static void ValidateListenerEventV2Id(Guid listenerEventV2Id)
        {
            Validate(
                message: "Listener event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(listenerEventV2Id),
                Parameter: nameof(ListenerEventV2.Id)));
        }

        private static void ValidateEventListenerV2Id(Guid eventListenerV2Id)
        {
            Validate(
                message: "Listener event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV2Id),
                Parameter: nameof(ListenerEventV2.EventListenerV2Id)));
        }

        private static dynamic IsNull(IEnumerable<Guid> value) => new
        {
            Condition = value is null,
            Message = "Value is required"
        };

        private static dynamic IsNull(IEnumerable<ListenerEventV2> value) => new
        {
            Condition = value is null,
            Message = "Value is required"
        };

        private static dynamic IsInvalid(int value) => new
        {
            Condition = value < 0,
            Message = "Value must be greater than or equal to 0"
        };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV2OrchestrationException =
                new InvalidListenerEventV2OrchestrationException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV2OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV2OrchestrationException.ThrowIfContainsErrors();
        }
    }
}
