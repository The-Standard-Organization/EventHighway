// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventV1ArchiveService
    {
        private async ValueTask ValidateEventV1ArchiveOnAddAsync(EventV1Archive eventV1Archive)
        {
            ValidateEventV1ArchiveIsNotNull(eventV1Archive);

            Validate(
                (Rule: IsInvalid(eventV1Archive.Id),
                Parameter: nameof(EventV1Archive.Id)),

                (Rule: IsInvalid(eventV1Archive.Content),
                Parameter: nameof(EventV1Archive.Content)),

                (Rule: IsInvalid(eventV1Archive.Type),
                Parameter: nameof(EventV1Archive.Type)),

                (Rule: IsInvalid(eventV1Archive.CreatedDate),
                Parameter: nameof(EventV1Archive.CreatedDate)),

                (Rule: IsInvalid(eventV1Archive.UpdatedDate),
                Parameter: nameof(EventV1Archive.UpdatedDate)),

                (Rule: IsInvalid(eventV1Archive.ArchivedDate),
                Parameter: nameof(EventV1Archive.ArchivedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventV1Archive.CreatedDate,
                    secondDate: eventV1Archive.UpdatedDate,
                    secondDateName: nameof(EventV1Archive.UpdatedDate)),

                Parameter: nameof(EventV1Archive.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventV1Archive.CreatedDate),
                Parameter: nameof(EventV1Archive.CreatedDate)),

            (Rule: IsInvalid(eventV1Archive.EventAddressId),
                Parameter: nameof(EventV1Archive.EventAddressId)));
        }

        private static void ValidateEventV1ArchiveIsNotNull(EventV1Archive eventV1Archive)
        {
            if (eventV1Archive is null)
            {
                throw new NullEventV1ArchiveException(
                    message: "Event listener is null.");
            }
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
                new InvalidEventV1ArchiveException(
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
