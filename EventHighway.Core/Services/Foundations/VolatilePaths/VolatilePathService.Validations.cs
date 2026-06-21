// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions;

namespace EventHighway.Core.Services.Foundations.VolatilePaths
{
    internal partial class VolatilePathService
    {
        private static void ValidateRemoveVolatilePaths(string content, string[] volatileContentPaths)
        {
            Validate(
                (Rule: IsInvalid(content), Parameter: nameof(content)),
                (Rule: IsInvalid(volatileContentPaths), Parameter: nameof(volatileContentPaths)));
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = text is null,
            Message = "Value is required"
        };

        private static dynamic IsInvalid(string[] array) => new
        {
            Condition = array is null,
            Message = "Value is required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidVolatilePathServiceException =
                new InvalidVolatilePathServiceException(
                    message: "Invalid volatile path service error occurred, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidVolatilePathServiceException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidVolatilePathServiceException.ThrowIfContainsErrors();
        }
    }
}
