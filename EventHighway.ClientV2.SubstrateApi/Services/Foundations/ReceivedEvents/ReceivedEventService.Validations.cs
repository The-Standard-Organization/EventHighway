// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents.Exceptions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents
{
    public partial class ReceivedEventService
    {
        private static void ValidateContent(string content)
        {
            ValidateContentIsNotNull(content);

            Validate(
                (Rule: IsInvalid(content), Parameter: nameof(content)));
        }

        private static void ValidateContentIsNotNull(string content)
        {
            if (content is null)
            {
                throw new NullReceivedEventException(
                    message: "Received event content is null.");
            }
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidReceivedEventException =
                new InvalidReceivedEventException(
                    message: "Received event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidReceivedEventException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidReceivedEventException.ThrowIfContainsErrors();
        }
    }
}
