// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.Events.V1
{
    internal partial class EventV1Service
    {
        private async ValueTask ValidateEventOnAddAsync(EventV1 @event)
        {
            ValidateEventV1IsNotNull(@event);

            Validate(
                (Rule: IsInvalid(@event.Id),
                Parameter: nameof(EventV1.Id)),

                (Rule: IsInvalid(@event.Content),
                Parameter: nameof(EventV1.Content)),

                (Rule: IsInvalid(@event.EventAddressId),
                Parameter: nameof(EventV1.EventAddressId)),

                (Rule: IsInvalid(@event.Type),
                Parameter: nameof(EventV1.Type)),

                (Rule: IsInvalid(@event.CreatedDate),
                Parameter: nameof(EventV1.CreatedDate)),

                (Rule: IsInvalid(@event.UpdatedDate),
                Parameter: nameof(EventV1.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: @event.CreatedDate,
                    secondDate: @event.UpdatedDate,
                    secondDateName: nameof(EventV1.UpdatedDate)),

                Parameter: nameof(EventV1.CreatedDate)),

                (Rule: await IsNotRecentAsync(@event.CreatedDate),
                Parameter: nameof(EventV1.CreatedDate)));
        }

        private async ValueTask ValidateEventOnModifyAsync(EventV1 @event)
        {
            ValidateEventV1IsNotNull(@event);

            Validate(
                (Rule: IsInvalid(@event.Id),
                Parameter: nameof(EventV1.Id)),

                (Rule: IsInvalid(@event.Content),
                Parameter: nameof(EventV1.Content)),

                (Rule: IsInvalid(@event.EventAddressId),
                Parameter: nameof(EventV1.EventAddressId)),

                (Rule: IsInvalid(@event.Type),
                Parameter: nameof(EventV1.Type)),

                (Rule: IsInvalid(@event.CreatedDate),
                Parameter: nameof(EventV1.CreatedDate)),

                (Rule: IsInvalid(@event.UpdatedDate),
                Parameter: nameof(EventV1.UpdatedDate)),

                (Rule: IsSameAs(
                    firstDate: @event.CreatedDate,
                    secondDate: @event.UpdatedDate,
                    secondDateName: nameof(EventV1.CreatedDate)),

                Parameter: nameof(EventV1.UpdatedDate)),

                (Rule: await IsNotRecentAsync(@event.UpdatedDate),
                Parameter: nameof(EventV1.UpdatedDate)));
        }

        private static void ValidateEventId(Guid eventId)
        {
            Validate(
                (Rule: IsInvalid(eventId),
                Parameter: nameof(EventV1.Id)));
        }

        private static void ValidateEventV1IsNotNull(EventV1 @event)
        {
            if (@event is null)
            {
                throw new NullEventV1Exception(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventAgainstStorage(
            EventV1 incomingEvent,
            EventV1 storageEvent)
        {
            ValidateEventExists(
                eventV1: storageEvent,
                eventV1Id: incomingEvent.Id);

            Validate(
                (Rule: IsNotSameAsStorage(
                    firstDate: incomingEvent.CreatedDate,
                    secondDate: storageEvent.CreatedDate),
                Parameter: nameof(EventV1.CreatedDate)),

                (Rule: IsEarlierThan(
                    firstDate: incomingEvent.UpdatedDate,
                    secondDate: storageEvent.UpdatedDate),

                Parameter: nameof(EventV1.UpdatedDate)));
        }

        private static void ValidateEventExists(
            EventV1 eventV1,
            Guid eventV1Id)
        {
            if (eventV1 is null)
            {
                throw new NotFoundEventV1Exception(

                    message: $"Could not find event " +
                        $"with id: {eventV1Id}.");
            }
        }

        private static dynamic IsNotSameAsStorage(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as storage."
            };

        private static dynamic IsEarlierThan(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate) => new
            {
                Condition = firstDate < secondDate,
                Message = $"Date is earlier than storage."
            };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
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

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsInvalidEnum(value) is true,
            Message = "Value is not recognized"
        };

        private static dynamic IsNotSameAs(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsSameAs(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}."
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
            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV1Exception.ThrowIfContainsErrors();
        }
    }
}
