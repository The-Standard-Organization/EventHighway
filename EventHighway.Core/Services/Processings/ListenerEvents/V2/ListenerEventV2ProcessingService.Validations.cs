// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V2
{
    internal partial class ListenerEventV2ProcessingService
    {
        private static void ValidateListenerEventV2IsNotNull(ListenerEventV2 listenerEventV2)
        {
            if (listenerEventV2 is null)
            {
                throw new NullListenerEventV2ProcessingException(
                    message: "Listener event is null.");
            }
        }

        private static void ValidateListenerEventV2sIsNotNull(IEnumerable<ListenerEventV2> listenerEventV2s)
        {
            if (listenerEventV2s is null)
            {
                throw new NullListenerEventV2ProcessingException(
                    message: "Listener event is null.");
            }
        }

        private static void ValidateListenerEventV2Id(Guid listenerEventV2Id)
        {
            Validate(
                (Rule: IsInvalid(listenerEventV2Id),
                Parameter: nameof(ListenerEventV2.Id)));
        }

        private static void ValidateEventListenerV2Id(Guid eventListenerV2Id)
        {
            Validate(
                (Rule: IsInvalid(eventListenerV2Id),
                Parameter: nameof(ListenerEventV2.EventListenerV2Id)));
        }

        private static void ValidateOnRetrieveBatchOfReplayListenerEventV2s(int take)
        {
            Validate(
                (Rule: IsInvalid(take), Parameter: "Take"));
        }

        private static void ValidateOnRetrieveBatchOfRetryListenerEventV2s(int take)
        {
            Validate(
                (Rule: IsInvalid(take), Parameter: "Take"));
        }

        private static void ValidateOnRetrieveBatchOfListenerEventV2sByEventIds(
            IEnumerable<Guid> eventIds,
            int take)
        {
            Validate(
                (Rule: IsInvalid(eventIds), Parameter: "EventIds"),
                (Rule: IsInvalid(take), Parameter: "Take"));
        }

        private static dynamic IsInvalid(IEnumerable<Guid> eventIds) => new
        {
            Condition = eventIds is null,
            Message = "Required"
        };

        private static dynamic IsInvalid(int take) => new
        {
            Condition = take < 0,
            Message = "Value must be greater than or equal to 0"
        };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV2ProcessingException =
                new InvalidListenerEventV2ProcessingException(
                    message: "Listener event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
