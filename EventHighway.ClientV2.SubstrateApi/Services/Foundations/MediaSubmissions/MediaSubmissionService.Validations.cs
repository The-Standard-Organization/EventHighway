// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApi.Models.Brokers.Configurations;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions.Exceptions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaSubmissions
{
    public partial class MediaSubmissionService
    {
        private static void ValidateContent(string content)
        {
            Validate(
                message: "Media submission is invalid, fix the errors and try again.",
                (Rule: IsInvalid(content), Parameter: nameof(content)));
        }

        // A missing key here is a deployment problem, not a user's: without them the send button
        // has nowhere to post and the chat has no credentials to show. Say which key is missing.
        private static void ValidateConfigurations(SubstrateApiConfigurations configurations)
        {
            Validate(
                message: "SubstrateApi configurations are invalid, fix the errors and try again.",

                (Rule: IsInvalid(configurations.SubmitUrl),
                Parameter: "SubstrateApi:SubmitUrl"),

                (Rule: IsInvalid(configurations.ParticipantId),
                Parameter: "SubstrateApi:ParticipantId"),

                (Rule: IsInvalid(configurations.ParticipantSecret),
                Parameter: "SubstrateApi:ParticipantSecret"));
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate(
            string message,
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidMediaSubmissionException =
                new InvalidMediaSubmissionException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidMediaSubmissionException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidMediaSubmissionException.ThrowIfContainsErrors();
        }
    }
}
