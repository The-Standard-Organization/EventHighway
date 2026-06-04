// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventArchiveV1Service
    {
        private async ValueTask ValidateEventArchiveV1OnAddAsync(EventArchiveV1 eventArchiveV1)
        {
            ValidateEventArchiveV1IsNotNull(eventArchiveV1);

            Validate(
                message: "Event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventArchiveV1.Id),
                Parameter: nameof(EventArchiveV1.Id)),

                (Rule: IsInvalid(eventArchiveV1.Content),
                Parameter: nameof(EventArchiveV1.Content)),

                (Rule: IsInvalid(eventArchiveV1.Type),
                Parameter: nameof(EventArchiveV1.Type)),

                (Rule: IsInvalid(eventArchiveV1.CreatedDate),
                Parameter: nameof(EventArchiveV1.CreatedDate)),

                (Rule: IsInvalid(eventArchiveV1.UpdatedDate),
                Parameter: nameof(EventArchiveV1.UpdatedDate)),

                (Rule: IsInvalid(eventArchiveV1.ArchivedDate),
                Parameter: nameof(EventArchiveV1.ArchivedDate)),

                (Rule: await IsNotRecentAsync(eventArchiveV1.ArchivedDate),
                Parameter: nameof(EventArchiveV1.ArchivedDate)),

                (Rule: IsInvalid(eventArchiveV1.EventAddressId),
                Parameter: nameof(EventArchiveV1.EventAddressId)));
        }

        private static void ValidateEventArchiveV1IsNotNull(EventArchiveV1 eventArchiveV1)
        {
            if (eventArchiveV1 is null)
            {
                throw new NullEventArchiveV1Exception(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateEventArchiveV1Id(Guid eventArchiveV1Id)
        {
            Validate(
                message: "Event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventArchiveV1Id),
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

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventArchiveV1Exception = new InvalidEventArchiveV1Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventArchiveV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventArchiveV1Exception.ThrowIfContainsErrors();
        }
    }
}