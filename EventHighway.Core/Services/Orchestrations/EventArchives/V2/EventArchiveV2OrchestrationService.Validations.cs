// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal partial class EventArchiveV2OrchestrationService
    {
        private static void ValidateEventArchiveV2(EventArchiveV2 eventArchiveV2)
        {
            ValidateEventArchiveV2IsNotNull(eventArchiveV2);
            ValidateListenerEventArchiveV2sAreNotNull(eventArchiveV2);
        }

        private static void ValidateEventArchiveV2IsNotNull(EventArchiveV2 eventArchiveV2)
        {
            if (eventArchiveV2 is null)
            {
                throw new NullEventArchiveV2OrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventArchiveV2sAreNotNull(EventArchiveV2 eventArchiveV2)
        {
            if (eventArchiveV2.ListenerEventArchiveV2s is null)
            {
                throw new NullListenerEventArchiveV2sOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }

        private void ValidateOnRetrieveNextPurgeBatchOfArchivedEventV2s(
         DateTimeOffset olderThan,
         BatchConfiguration batchConfiguration)
        {
            Validate(
                message: "Event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(olderThan),
                Parameter: nameof(olderThan)),

                (Rule: IsInvalid(batchConfiguration),
                Parameter: nameof(BatchConfiguration)));
        }

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required."
        };

        private static dynamic IsInvalid(object @object) => new
        {
            Condition = @object == null,
            Message = "Required."
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventArchiveV2OrchestrationException =
                new InvalidEventArchiveV2OrchestrationException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventArchiveV2OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventArchiveV2OrchestrationException.ThrowIfContainsErrors();
        }
    }
}
