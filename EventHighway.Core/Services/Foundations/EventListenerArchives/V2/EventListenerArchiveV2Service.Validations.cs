// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal partial class EventListenerArchiveV2Service
    {
        private async ValueTask ValidateEventListenerArchiveV2OnAddAsync(
            EventListenerArchiveV2 eventListenerArchiveV2)
        {
            ValidateEventListenerArchiveV2IsNotNull(eventListenerArchiveV2);

            Validate(
                message: "Event listener archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerArchiveV2.Id),
                Parameter: nameof(EventListenerArchiveV2.Id)),

                (Rule: IsInvalid(eventListenerArchiveV2.Name),
                Parameter: nameof(EventListenerArchiveV2.Name)),

                (Rule: IsInvalid(eventListenerArchiveV2.Description),
                Parameter: nameof(EventListenerArchiveV2.Description)),

                (Rule: IsInvalid(eventListenerArchiveV2.HandlerId),
                Parameter: nameof(EventListenerArchiveV2.HandlerId)),

                (Rule: IsInvalid(eventListenerArchiveV2.EventListenerId),
                Parameter: nameof(EventListenerArchiveV2.EventListenerId)),

                (Rule: IsInvalid(eventListenerArchiveV2.EventAddressId),
                Parameter: nameof(EventListenerArchiveV2.EventAddressId)),

                (Rule: IsInvalid(eventListenerArchiveV2.EventArchiveV2Id),
                Parameter: nameof(EventListenerArchiveV2.EventArchiveV2Id)),

                (Rule: IsInvalid(eventListenerArchiveV2.CreatedDate),
                Parameter: nameof(EventListenerArchiveV2.CreatedDate)),

                (Rule: IsInvalid(eventListenerArchiveV2.UpdatedDate),
                Parameter: nameof(EventListenerArchiveV2.UpdatedDate)),

                (Rule: IsInvalid(eventListenerArchiveV2.ArchivedDate),
                Parameter: nameof(EventListenerArchiveV2.ArchivedDate)),

                (Rule: await IsNotRecentAsync(eventListenerArchiveV2.ArchivedDate),
                Parameter: nameof(EventListenerArchiveV2.ArchivedDate)));
        }

        private static void ValidateEventListenerArchiveV2sIsNotNull(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s)
        {
            if (eventListenerArchiveV2s is null)
            {
                throw new NullEventListenerArchiveV2Exception(
                    message: "Event listener archive is null.");
            }
        }

        private static void ValidateEventListenerArchiveV2IsNotNull(
            EventListenerArchiveV2 eventListenerArchiveV2)
        {
            if (eventListenerArchiveV2 is null)
            {
                throw new NullEventListenerArchiveV2Exception(
                    message: "Event listener archive is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
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
            var invalidEventListenerArchiveV2Exception =
                new InvalidEventListenerArchiveV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerArchiveV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerArchiveV2Exception.ThrowIfContainsErrors();
        }
    }
}
