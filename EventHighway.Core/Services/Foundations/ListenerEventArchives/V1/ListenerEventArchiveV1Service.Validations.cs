// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V1
{
    internal partial class ListenerEventArchiveV1Service
    {
        private async ValueTask ValidateListenerEventArchiveOnAddAsync(
            ListenerEventArchiveV1 listenerEventArchive)
        {
            ValidateListenerEventArchiveIsNotNull(listenerEventArchive);

            Validate(
                (Rule: IsInvalid(listenerEventArchive.Id),
                Parameter: nameof(ListenerEventArchiveV1.Id)),

                (Rule: IsInvalid(listenerEventArchive.EventId),
                Parameter: nameof(ListenerEventArchiveV1.EventId)),

                (Rule: IsInvalid(listenerEventArchive.EventAddressId),
                Parameter: nameof(ListenerEventArchiveV1.EventAddressId)),

                (Rule: IsInvalid(listenerEventArchive.EventListenerId),
                Parameter: nameof(ListenerEventArchiveV1.EventListenerId)),

                (Rule: IsInvalid(listenerEventArchive.Status),
                Parameter: nameof(ListenerEventArchiveV1.Status)),

                (Rule: IsInvalid(listenerEventArchive.CreatedDate),
                Parameter: nameof(ListenerEventArchiveV1.CreatedDate)),

                (Rule: IsInvalid(listenerEventArchive.UpdatedDate),
                Parameter: nameof(ListenerEventArchiveV1.UpdatedDate)),

                (Rule: IsInvalid(listenerEventArchive.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV1.ArchivedDate)),

                (Rule: await IsNotRecentAsync(listenerEventArchive.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV1.ArchivedDate)));
        }

        private static void ValidateListenerEventArchiveIsNotNull(
            ListenerEventArchiveV1 listenerEventArchive)
        {
            if (listenerEventArchive is null)
            {
                throw new NullListenerEventArchiveV1Exception(
                    message: "Listener event archive is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsInvalidEnum(value) is true,
            Message = "Value is not recognized"
        };

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date) => new
        {
            Condition = await IsDateNotRecentAsync(date),
            Message = "Date is not recent"
        };

        private async ValueTask<bool> IsDateNotRecentAsync(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            TimeSpan timeDifference = currentDateTime.Subtract(value: date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static bool IsInvalidEnum<T>(T enumValue)
        {
            bool isDefined = Enum.IsDefined(
                enumType: typeof(T),
                value: enumValue);

            return isDefined is false;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventArchiveV1Exception =
                new InvalidListenerEventArchiveV1Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventArchiveV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventArchiveV1Exception.ThrowIfContainsErrors();
        }
    }
}
