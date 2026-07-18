// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService
    {
        private static void ValidateOnPurgeEventArchiveV2s(DateTimeOffset olderThan)
        {
            Validate(
                (Rule: IsInvalid(olderThan), Parameter: "OlderThan"));
        }

        private static void ValidateOnPurgeFromConfiguration(int retentionDays)
        {
            Validate(
                (Rule: IsInvalidRetentionDays(retentionDays), Parameter: "RetentionDays"));
        }

        private static dynamic IsInvalidRetentionDays(int retentionDays) => new
        {
            Condition = retentionDays < 1,
            Message = "Retention days must be greater than zero to avoid purging all archived events."
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidArchivingEventV2CoordinationException =
                new InvalidArchivingEventV2CoordinationException(
                    message: "Archiving event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidArchivingEventV2CoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidArchivingEventV2CoordinationException.ThrowIfContainsErrors();
        }
    }
}
