// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventListeners.V2
{
    internal partial class EventListenerV2Service
    {
        private async ValueTask ValidateEventListenerV2OnAddAsync(EventListenerV2 eventListenerV2)
        {
            ValidateEventListenerV2IsNotNull(eventListenerV2);

            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV2.Id),
                Parameter: nameof(EventListenerV2.Id)),

                (Rule: IsInvalid(eventListenerV2.Name),
                Parameter: nameof(EventListenerV2.Name)),

                (Rule: IsExceedingLengthOf(eventListenerV2.Name, 450),
                Parameter: nameof(EventListenerV2.Name)),

                (Rule: IsInvalid(eventListenerV2.Description),
                Parameter: nameof(EventListenerV2.Description)),

                (Rule: IsInvalid(eventListenerV2.HandlerName),
                Parameter: nameof(EventListenerV2.HandlerName)),

                (Rule: IsExceedingLengthOf(eventListenerV2.FilterCriteria, 4000),
                Parameter: nameof(EventListenerV2.FilterCriteria)),

                (Rule: IsInvalid(eventListenerV2.EventAddressV2Id),
                Parameter: nameof(EventListenerV2.EventAddressV2Id)),

                (Rule: IsInvalid(eventListenerV2.EventParticipantV2Id),
                Parameter: nameof(EventListenerV2.EventParticipantV2Id)),

                (Rule: IsInvalid(eventListenerV2.CreatedDate),
                Parameter: nameof(EventListenerV2.CreatedDate)),

                (Rule: IsInvalid(eventListenerV2.UpdatedDate),
                Parameter: nameof(EventListenerV2.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventListenerV2.CreatedDate,
                    secondDate: eventListenerV2.UpdatedDate,
                    secondDateName: nameof(EventListenerV2.UpdatedDate)),

                Parameter: nameof(EventListenerV2.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventListenerV2.CreatedDate),
                Parameter: nameof(EventListenerV2.CreatedDate)));
        }

        private static void ValidateEventListenerV2Id(Guid eventListenerV2Id)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV2Id),
                Parameter: nameof(EventListenerV2.Id)));
        }

        private static void ValidateEventListenerV2Exists(
            EventListenerV2 eventListenerV2,
            Guid eventListenerV2Id)
        {
            if (eventListenerV2 is null)
            {
                throw new NotFoundEventListenerV2Exception(

                    message: $"Could not find event listener " +
                        $"with id: {eventListenerV2Id}.");
            }
        }

        private static void ValidateEventListenerV2IsNotNull(EventListenerV2 eventListenerV2)
        {
            if (eventListenerV2 is null)
            {
                throw new NullEventListenerV2Exception(
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

        private static dynamic IsExceedingLengthOf(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

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
            var invalidEventListenerV2Exception = new InvalidEventListenerV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerV2Exception.ThrowIfContainsErrors();
        }
    }
}
