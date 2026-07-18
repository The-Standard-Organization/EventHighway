// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
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

        private static void ValidateListenerEventV2Query(
            ListenerEventV2Query listenerEventV2Query)
        {
            ValidateListenerEventV2QueryIsNotNull(listenerEventV2Query);

            ValidateQuery(
                (Rule: IsNegative(listenerEventV2Query.Skip),
                Parameter: nameof(ListenerEventV2Query.Skip)),

                (Rule: IsOutOfRange(listenerEventV2Query.Take),
                Parameter: nameof(ListenerEventV2Query.Take)),

                (Rule: IsBefore(
                    firstDate: listenerEventV2Query.CreatedTo,
                    secondDate: listenerEventV2Query.CreatedFrom,
                    secondDateName: nameof(ListenerEventV2Query.CreatedFrom)),
                Parameter: nameof(ListenerEventV2Query.CreatedTo)));
        }

        private static void ValidateListenerEventV2QueryIsNotNull(
            ListenerEventV2Query listenerEventV2Query)
        {
            if (listenerEventV2Query is null)
            {
                throw new NullListenerEventV2QueryOrchestrationException(
                    message: "Listener event query is null.");
            }
        }

        private static dynamic IsNegative(int value) => new
        {
            Condition = value < 0,
            Message = "Value must be zero or greater"
        };

        private static dynamic IsOutOfRange(int value) => new
        {
            Condition = value < 1 || value > 1000,
            Message = "Value must be between 1 and 1000"
        };

        private static dynamic IsBefore(
            DateTimeOffset? firstDate,
            DateTimeOffset? secondDate,
            string secondDateName) => new
            {
                Condition = firstDate is not null
                    && secondDate is not null
                    && firstDate < secondDate,

                Message = $"Date must be after {secondDateName}"
            };

        private static void ValidateQuery(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV2QueryOrchestrationException =
                new InvalidListenerEventV2QueryOrchestrationException(
                    message: "Listener event query is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV2QueryOrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV2QueryOrchestrationException.ThrowIfContainsErrors();
        }
    }
}
