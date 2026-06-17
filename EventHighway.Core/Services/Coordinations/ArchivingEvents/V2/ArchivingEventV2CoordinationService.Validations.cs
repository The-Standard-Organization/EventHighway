// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService
    {

        private void ValidateIsOlderThan(DateTimeOffset olderThan)
        {
            Validate(
                message: "Archiving event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(olderThan),
                Parameter: nameof(olderThan)));
        }

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required."
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidArchivingEventV2CoordinationException =
                new InvalidArchivingEventV2CoordinationException(message);

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
