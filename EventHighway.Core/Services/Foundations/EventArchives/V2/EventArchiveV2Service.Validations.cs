// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V2
{
    internal partial class EventArchiveV2Service
    {
        private async ValueTask ValidateEventArchiveV2OnAddAsync(EventArchiveV2 eventArchiveV2)
        {
            ValidateEventArchiveV2IsNotNull(eventArchiveV2);

            Validate(
                message: "Event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventArchiveV2.Id),
                Parameter: nameof(EventArchiveV2.Id)),

                (Rule: IsInvalid(eventArchiveV2.Content),
                Parameter: nameof(EventArchiveV2.Content)),

                (Rule: IsInvalid(eventArchiveV2.ContentHash),
                Parameter: nameof(EventArchiveV2.ContentHash)),

                (Rule: IsInvalid(eventArchiveV2.EventName),
                Parameter: nameof(EventArchiveV2.EventName)),

                (Rule: IsInvalid(eventArchiveV2.Type),
                Parameter: nameof(EventArchiveV2.Type)),

                (Rule: IsInvalid(eventArchiveV2.CreatedDate),
                Parameter: nameof(EventArchiveV2.CreatedDate)),

                (Rule: IsInvalid(eventArchiveV2.UpdatedDate),
                Parameter: nameof(EventArchiveV2.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventArchiveV2.CreatedDate,
                    secondDate: eventArchiveV2.UpdatedDate,
                    secondDateName: nameof(EventArchiveV2.UpdatedDate)),
                Parameter: nameof(EventArchiveV2.CreatedDate)),

                (Rule: IsInvalid(eventArchiveV2.ArchivedDate),
                Parameter: nameof(EventArchiveV2.ArchivedDate)),

                (Rule: await IsNotRecentAsync(eventArchiveV2.ArchivedDate),
                Parameter: nameof(EventArchiveV2.ArchivedDate)),

                (Rule: IsInvalid(eventArchiveV2.EventAddressV2Id),
                Parameter: nameof(EventArchiveV2.EventAddressV2Id)));
        }

        private static void ValidateEventArchiveV2IsNotNull(EventArchiveV2 eventArchiveV2)
        {
            if (eventArchiveV2 is null)
            {
                throw new NullEventArchiveV2Exception(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateEventArchiveV2sIsNotNull(IEnumerable<EventArchiveV2> eventArchiveV2s)
        {
            if (eventArchiveV2s is null)
            {
                throw new NullEventArchiveV2Exception(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateEventArchiveV2Exists(EventArchiveV2 eventArchiveV2, Guid eventArchiveV2Id)
        {
            if (eventArchiveV2 is null)
            {
                throw new NotFoundEventArchiveV2Exception(
                    message: $"Could not find event archive with id: {eventArchiveV2Id}.");
            }
        }

        private static void ValidateEventArchiveV2Id(Guid eventArchiveV2Id)
        {
            Validate(
                message: "Event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventArchiveV2Id),
                Parameter: nameof(EventArchiveV2.Id)));
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
            var invalidEventArchiveV2Exception = new InvalidEventArchiveV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventArchiveV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventArchiveV2Exception.ThrowIfContainsErrors();
        }
    }
}
