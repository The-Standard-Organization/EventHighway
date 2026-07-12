// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService
    {
        private static void ValidateOnRetrieveHealthReport(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd)
        {
            Validate(
                (Rule: IsInvalid(windowStart), Parameter: "WindowStart"),
                (Rule: IsMissingForCustomPeriod(period, windowEnd), Parameter: "WindowEnd"));
        }

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static dynamic IsMissingForCustomPeriod(TrafficPeriodV2 period, DateTimeOffset? windowEnd) => new
        {
            Condition = period == TrafficPeriodV2.Custom && windowEnd is null,
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
