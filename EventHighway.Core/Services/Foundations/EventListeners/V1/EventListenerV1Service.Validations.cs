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
        private async ValueTask ValidateEventListenerV1OnAddAsync(EventListenerV1 eventListener)
        {
            ValidateEventListenerV1IsNotNull(eventListener);

            Validate(
                (Rule: IsInvalid(eventListener.Id),
                Parameter: nameof(EventListenerV1.Id)),

                (Rule: IsInvalid(eventListener.Name),
                Parameter: nameof(EventListenerV1.Name)),

                (Rule: IsInvalid(eventListener.Description),
                Parameter: nameof(EventListenerV1.Description)),

                (Rule: IsInvalid(eventListener.HeaderSecret),
                Parameter: nameof(EventListenerV1.HeaderSecret)),

                (Rule: IsInvalid(eventListener.Endpoint),
                Parameter: nameof(EventListenerV1.Endpoint)),

                (Rule: IsInvalid(eventListener.EventAddressId),
                Parameter: nameof(EventListenerV1.EventAddressId)),

                (Rule: IsInvalid(eventListener.CreatedDate),
                Parameter: nameof(EventListenerV1.CreatedDate)),

                (Rule: IsInvalid(eventListener.UpdatedDate),
                Parameter: nameof(EventListenerV1.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventListener.CreatedDate,
                    secondDate: eventListener.UpdatedDate,
                    secondDateName: nameof(EventListenerV1.UpdatedDate)),

                Parameter: nameof(EventListenerV1.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventListener.CreatedDate),
                Parameter: nameof(EventListenerV1.CreatedDate)));
        }

        private static void ValidateEventListenerId(Guid eventListenerId)
        {
            Validate(
                (Rule: IsInvalid(eventListenerId),
                Parameter: nameof(EventListenerV1.Id)));
        }

        private static void ValidateEventListenerExists(
            EventListenerV1 eventListener,
            Guid eventListenerId)
        {
            if (eventListener is null)
            {
                throw new NotFoundEventListenerV1Exception(

                    message: $"Could not find event listener " +
                        $"with id: {eventListenerId}.");
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

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventListenerV1Exception =
                new InvalidEventListenerV1Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

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
