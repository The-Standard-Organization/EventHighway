// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService
    {
        private static void ValidateOnRetrieveHealthReport(DateTimeOffset windowStart)
        {
            Validate(
                (Rule: IsInvalid(windowStart), Parameter: "WindowStart"));
        }

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidHealthV2CoordinationException =
                new InvalidHealthV2CoordinationException(
                    message: "Health coordination is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidHealthV2CoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidHealthV2CoordinationException.ThrowIfContainsErrors();
        }
    }
}
