// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.Events.V2
{
    internal partial class EventV2ProcessingService
    {
        private static void ValidateEventV2IsNotNull(EventV2 eventV2)
        {
            if (eventV2 is null)
            {
                throw new NullEventV2ProcessingException(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventV2sIsNotNull(IEnumerable<EventV2> eventV2s)
        {
            if (eventV2s is null)
            {
                throw new NullEventV2ProcessingException(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventV2Id(Guid eventV2Id)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventV2Id),
                Parameter: nameof(EventV2.Id)));
        }

        private static void ValidateOnRetrieveBatchOfDeadEventV2s(int take)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(take),
                Parameter: nameof(take)));
        }

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

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV2ProcessingException =
                new InvalidEventV2ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
