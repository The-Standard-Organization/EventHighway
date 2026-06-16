// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2ProcessingService
    {
        private void ValidateListenerEventArchiveV2(ListenerEventArchiveV2 listenerEventArchiveV2)
        {
            if (listenerEventArchiveV2 is null)
            {
                throw new NullListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is null.");
            }
        }

        private void ValidateOnRetrieveNextPurgeBatchOfArchivedEventV2s(
            DateTimeOffset olderThan,
            BatchConfiguration batchConfiguration)
        {
            Validate(
                message: "Listener event archive is invalid, fix the errors and try again.",

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
            var invalidListenerEventArchiveV2ProcessingException =
                new InvalidListenerEventArchiveV2ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventArchiveV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventArchiveV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
