// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventListeners.V1
{
    internal partial class EventListenerV1Service
    {
        private async ValueTask ValidateEventListenerV1OnAddAsync(EventListenerV1 eventListenerV1)
        {
            ValidateEventListenerV1IsNotNull(eventListenerV1);

            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV1.Id),
                Parameter: nameof(EventListenerV1.Id)),

                (Rule: IsInvalid(eventListenerV1.Name),
                Parameter: nameof(EventListenerV1.Name)),

                (Rule: IsInvalid(eventListenerV1.Description),
                Parameter: nameof(EventListenerV1.Description)),

                (Rule: IsInvalid(eventListenerV1.HeaderSecret),
                Parameter: nameof(EventListenerV1.HeaderSecret)),

                (Rule: IsInvalid(eventListenerV1.Endpoint),
                Parameter: nameof(EventListenerV1.Endpoint)),

                (Rule: IsInvalid(eventListenerV1.EventAddressId),
                Parameter: nameof(EventListenerV1.EventAddressId)),

                (Rule: IsInvalid(eventListenerV1.CreatedDate),
                Parameter: nameof(EventListenerV1.CreatedDate)),

                (Rule: IsInvalid(eventListenerV1.UpdatedDate),
                Parameter: nameof(EventListenerV1.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventListenerV1.CreatedDate,
                    secondDate: eventListenerV1.UpdatedDate,
                    secondDateName: nameof(EventListenerV1.UpdatedDate)),

                Parameter: nameof(EventListenerV1.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventListenerV1.CreatedDate),
                Parameter: nameof(EventListenerV1.CreatedDate)));
        }

        private static void ValidateEventListenerV1Id(Guid eventListenerV1Id)
        {
            Validate(
                message: "Event listener is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventListenerV1Id),
                Parameter: nameof(EventListenerV1.Id)));
        }

        private static void ValidateEventListenerV1Exists(
            EventListenerV1 eventListenerV1,
            Guid eventListenerV1Id)
        {
            if (eventListenerV1 is null)
            {
                throw new NotFoundEventListenerV1Exception(

                    message: $"Could not find event listener " +
                        $"with id: {eventListenerV1Id}.");
            }
        }

        private static void ValidateEventListenerV1IsNotNull(EventListenerV1 eventListenerV1)
        {
            if (eventListenerV1 is null)
            {
                throw new NullEventListenerV1Exception(
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
            var invalidEventListenerV1Exception = new InvalidEventListenerV1Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventListenerV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventListenerV1Exception.ThrowIfContainsErrors();
        }
    }
}
