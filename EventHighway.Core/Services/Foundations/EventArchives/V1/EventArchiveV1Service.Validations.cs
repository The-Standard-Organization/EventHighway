// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventArchiveV1Service
    {
        private async ValueTask ValidateEventArchiveOnAddAsync(EventArchiveV1 eventV1Archive)
        {
            ValidateEventArchiveIsNotNull(eventV1Archive);

            Validate(
                (Rule: IsInvalid(eventV1Archive.Id),
                Parameter: nameof(EventArchiveV1.Id)),

                (Rule: IsInvalid(eventV1Archive.Content),
                Parameter: nameof(EventArchiveV1.Content)),

                (Rule: IsInvalid(eventV1Archive.Type),
                Parameter: nameof(EventArchiveV1.Type)),

                (Rule: IsInvalid(eventV1Archive.CreatedDate),
                Parameter: nameof(EventArchiveV1.CreatedDate)),

                (Rule: IsInvalid(eventV1Archive.UpdatedDate),
                Parameter: nameof(EventArchiveV1.UpdatedDate)),

                (Rule: IsInvalid(eventV1Archive.ArchivedDate),
                Parameter: nameof(EventArchiveV1.ArchivedDate)),

                (Rule: await IsNotRecentAsync(eventV1Archive.ArchivedDate),
                Parameter: nameof(EventArchiveV1.ArchivedDate)),

                (Rule: IsInvalid(eventV1Archive.EventAddressId),
                Parameter: nameof(EventArchiveV1.EventAddressId)));
        }

        private static void ValidateEventArchiveIsNotNull(EventArchiveV1 eventArchive)
        {
            if (eventArchive is null)
            {
                throw new NullEventArchiveV1Exception(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateEventArchiveId(Guid eventArchiveId)
        {
            Validate(
                (Rule: IsInvalid(eventArchiveId),
                Parameter: nameof(EventArchiveV1.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(value: text),
            Message = "Required"
        };

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsInvalidEnum(value) is true,
            Message = "Value is not recognized"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static bool IsInvalidEnum<T>(T enumValue)
        {
            bool isDefined = Enum.IsDefined(
                enumType: typeof(T),
                value: enumValue);

            return isDefined is false;
        }

        private static dynamic IsNotSameAs(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
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

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV1ArchiveException =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV1ArchiveException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV1ArchiveException.ThrowIfContainsErrors();
        }
    }
}
