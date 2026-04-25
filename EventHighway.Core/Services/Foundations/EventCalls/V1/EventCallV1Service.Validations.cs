// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventCalls.V1
{
    internal partial class EventCallV1Service
    {
        private void ValidateEventCallOnRun(EventCallV1 eventCall)
        {
            ValidateEventCallIsNotNull(eventCall);

            Validate(
                (Rule: IsInvalid(eventCall.Endpoint),
                Parameter: nameof(EventCallV1.Endpoint)),

                (Rule: IsInvalid(eventCall.Content),
                Parameter: nameof(EventCallV1.Content)));
        }

        private static void ValidateEventCallIsNotNull(EventCallV1 eventCall)
        {
            if (eventCall is null)
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
