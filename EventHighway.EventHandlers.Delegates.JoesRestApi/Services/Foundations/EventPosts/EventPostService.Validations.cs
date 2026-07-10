// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Brokers.Configurations;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Foundations.EventPosts.Exceptions;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Services.Foundations.EventPosts
{
    internal partial class EventPostService
    {
        private static void ValidateContent(string content)
        {
            Validate(
                message: "Event post params are invalid, fix the errors and try again.",

                (Rule: IsInvalid(content),
                Parameter: "content",
                Message: "Text required"));
        }

        private static void ValidateConfigurations(JoesRestApiConfigurations configurations)
        {
            Validate(
                message: "Joes REST API configurations are invalid, fix the errors and try again.",

                (Rule: IsInvalid(configurations.Url),
                Parameter: "JoesRestApi:Url",
                Message: "Text required"),

                (Rule: IsInvalid(configurations.Secret),
                Parameter: "JoesRestApi:Secret",
                Message: "Text required"));
        }

        private static bool IsInvalid(string text) =>
            string.IsNullOrWhiteSpace(text);

        private static void Validate(
            string message,
            params (bool Rule, string Parameter, string Message)[] validations)
        {
            var invalidEventPostException =
                new InvalidEventPostException(message);

            foreach ((bool rule, string parameter, string errorMessage) in validations)
            {
                if (rule)
                {
                    invalidEventPostException.UpsertDataList(
                        key: parameter,
                        value: errorMessage);
                }
            }

            invalidEventPostException.ThrowIfContainsErrors();
        }
    }
}
