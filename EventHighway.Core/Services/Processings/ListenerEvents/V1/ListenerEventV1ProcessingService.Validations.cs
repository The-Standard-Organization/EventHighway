// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V1
{
    internal partial class ListenerEventV1ProcessingService
    {
        private static void ValidateListenerEventIsNotNull(ListenerEventV1 listenerEvent)
        {
            if (listenerEvent is null)
            {
                throw new NullListenerEventV1ProcessingException(
                    message: "Listener event is null.");
            }
        }

        private static void ValidateListenerEventId(Guid listenerEventId)
        {
            Validate(
                (Rule: IsInvalid(listenerEventId),
                Parameter: nameof(ListenerEventV1.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV1ProcessingException =
                new InvalidListenerEventV1ProcessingException(
                    message: "Listener event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV1ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV1ProcessingException.ThrowIfContainsErrors();
        }
    }
}
