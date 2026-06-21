// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.Events.V2
{
    internal partial class EventV2Service
    {
        private async ValueTask ValidateEventV2OnAddAsync(EventV2 eventV2)
        {
            ValidateEventV2IsNotNull(eventV2);

            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventV2.Id),
                Parameter: nameof(EventV2.Id)),

                (Rule: IsInvalid(eventV2.Content),
                Parameter: nameof(EventV2.Content)),

                (Rule: IsInvalid(eventV2.EventName),
                Parameter: nameof(EventV2.EventName)),

                (Rule: IsExceedingLengthOf(eventV2.EventName, 450),
                Parameter: nameof(EventV2.EventName)),

                (Rule: IsInvalid(eventV2.EventAddressId),
                Parameter: nameof(EventV2.EventAddressId)),

                (Rule: IsInvalid(eventV2.Type),
                Parameter: nameof(EventV2.Type)),

                (Rule: IsInvalid(eventV2.CreatedDate),
                Parameter: nameof(EventV2.CreatedDate)),

                (Rule: IsInvalid(eventV2.UpdatedDate),
                Parameter: nameof(EventV2.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventV2.CreatedDate,
                    secondDate: eventV2.UpdatedDate,
                    secondDateName: nameof(EventV2.UpdatedDate)),

                Parameter: nameof(EventV2.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventV2.CreatedDate),
                Parameter: nameof(EventV2.CreatedDate)));
        }

        private async ValueTask ValidateEventV2OnModifyAsync(EventV2 eventV2)
        {
            ValidateEventV2IsNotNull(eventV2);

            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventV2.Id),
                Parameter: nameof(EventV2.Id)),

                (Rule: IsInvalid(eventV2.Content),
                Parameter: nameof(EventV2.Content)),

                (Rule: IsInvalid(eventV2.EventName),
                Parameter: nameof(EventV2.EventName)),

                (Rule: IsExceedingLengthOf(eventV2.EventName, 450),
                Parameter: nameof(EventV2.EventName)),

                (Rule: IsInvalid(eventV2.EventAddressId),
                Parameter: nameof(EventV2.EventAddressId)),

                (Rule: IsInvalid(eventV2.Type),
                Parameter: nameof(EventV2.Type)),

                (Rule: IsInvalid(eventV2.CreatedDate),
                Parameter: nameof(EventV2.CreatedDate)),

                (Rule: IsInvalid(eventV2.UpdatedDate),
                Parameter: nameof(EventV2.UpdatedDate)),

                (Rule: IsSameAs(
                    firstDate: eventV2.CreatedDate,
                    secondDate: eventV2.UpdatedDate,
                    secondDateName: nameof(EventV2.CreatedDate)),

                Parameter: nameof(EventV2.UpdatedDate)),

                (Rule: await IsNotRecentAsync(eventV2.UpdatedDate),
                Parameter: nameof(EventV2.UpdatedDate)));
        }

        private static void ValidateEventV2Id(Guid eventV2Id)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventV2Id),
                Parameter: nameof(EventV2.Id)));
        }

        private static void ValidateOnRetrieveEventV2CountBySignature(EventV2 eventV2)
        {
            ValidateEventV2IsNotNull(eventV2);
        }

        private static void ValidateOnRemoveVolatilePaths(EventV2 eventV2)
        {
            ValidateEventV2IsNotNull(eventV2);
        }

        private static void ValidateOnRemoveVolatilePathsWithConfig(
            EventV2 eventV2,
            string[] volatileContentPaths)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventV2.EventAddressId),
                Parameter: nameof(EventV2.EventAddressId)),

                (Rule: IsInvalid(volatileContentPaths),
                Parameter: nameof(VolatilePaths.VolatileContentPaths)));
        }

        private static void ValidateEventV2IsNotNull(EventV2 eventV2)
        {
            if (eventV2 is null)
            {
                throw new NullEventV2Exception(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventV2sIsNotNull(IEnumerable<EventV2> eventV2s)
        {
            if (eventV2s is null)
            {
                throw new NullEventV2Exception(
                    message: "Event is null.");
            }
        }

        private static void ValidateEventV2AgainstStorage(
            EventV2 incomingEventV2,
            EventV2 storageEventV2)
        {
            ValidateEventV2Exists(
                eventV2: storageEventV2,
                eventV2Id: incomingEventV2.Id);

            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsNotSameAsStorage(
                    firstDate: incomingEventV2.CreatedDate,
                    secondDate: storageEventV2.CreatedDate),
                Parameter: nameof(EventV2.CreatedDate)),

                (Rule: IsEarlierThan(
                    firstDate: incomingEventV2.UpdatedDate,
                    secondDate: storageEventV2.UpdatedDate),

                Parameter: nameof(EventV2.UpdatedDate)));
        }

        private static void ValidateEventV2Exists(
            EventV2 eventV2,
            Guid eventV2Id)
        {
            if (eventV2 is null)
            {
                throw new NotFoundEventV2Exception(

                    message: $"Could not find event " +
                        $"with id: {eventV2Id}.");
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

        private static dynamic IsExceedingLengthOf(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

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

        private static dynamic IsInvalid(string[] array) => new
        {
            Condition = array is null,
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

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventV2Exception = new InvalidEventV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventV2Exception.ThrowIfContainsErrors();
        }
    }
}
