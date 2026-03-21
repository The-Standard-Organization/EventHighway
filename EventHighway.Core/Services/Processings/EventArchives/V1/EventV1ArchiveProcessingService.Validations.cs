// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventV1ArchiveProcessingService
    {
        private static void ValidateEventV1ArchiveIsNotNull(EventV1Archive eventV1Archive)
        {
            if (eventV1Archive is null)
            {
                throw new NullEventV1ArchiveProcessingException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateEventV1ArchiveId(Guid courseId)
        {
            Validate(
                (Rule: IsInvalid(courseId),
                Parameter: nameof(EventV1Archive.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV1ArchiveProcessingException =
                new InvalidEventV1ArchiveProcessingException(
                    message: "Event archive is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV1ArchiveProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV1ArchiveProcessingException.ThrowIfContainsErrors();
        }
    }
}
