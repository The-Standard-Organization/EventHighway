// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V1
{
    internal partial class ListenerEventV1ProcessingService
    {
        private static void ValidateListenerEventV1IsNotNull(ListenerEventV1 listenerEventV1)
        {
            if (listenerEventV1 is null)
            {
                throw new NullListenerEventV1ProcessingException(
                    message: "Listener event is null.");
            }
        }

        private static void ValidateListenerEventV1Id(Guid listenerEventV1Id)
        {
            Validate(
                message: "Listener event is invalid, fix the errors and try again.",
                (Rule: IsInvalid(listenerEventV1Id),
                Parameter: nameof(ListenerEventV1.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV1ProcessingException =
                new InvalidListenerEventV1ProcessingException(
                    message: message);

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
