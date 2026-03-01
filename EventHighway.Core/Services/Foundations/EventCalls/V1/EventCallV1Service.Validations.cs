// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Net.Http;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventCalls.V1
{
    internal partial class EventCallV1Service
    {
        private void ValidateEventCallV1OnRun(EventCallV1 eventCallV1)
        {
            ValidateEventCallV1IsNotNull(eventCallV1);

            Validate(
                (Rule: IsInvalid(eventCallV1.Endpoint),
                Parameter: nameof(EventCallV1.Endpoint)),

                (Rule: IsInvalid(eventCallV1.Content),
                Parameter: nameof(EventCallV1.Content)));
        }

        private static void ValidateEventCallV1IsNotNull(EventCallV1 eventCallV1)
        {
            if (eventCallV1 is null)
            {
                throw new NullEventCallV1Exception(
                    message: "Event call is null.");
            }
        }

        private static void ValidateHttpResponseMessageIsNotNull(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage is null)
            {
                throw new NullHttpResponseMessageException(
                    message: "Http response message is null.");
            }
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventCallV1Exception =
                new InvalidEventCallV1Exception(
                    message: "Event call is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventCallV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventCallV1Exception.ThrowIfContainsErrors();
        }
    }
}
