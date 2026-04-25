// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListernEvents.V1
{
    internal partial class ListenerEventV1Service
    {
        private async ValueTask ValidateListenerEventOnAddAsync(ListenerEventV1 listenerEvent)
        {
            ValidateListenerEventIsNotNull(listenerEvent);

            Validate(
                (Rule: IsInvalid(listenerEvent.Id),
                Parameter: nameof(ListenerEventV1.Id)),

                (Rule: IsInvalid(listenerEvent.EventId),
                Parameter: nameof(ListenerEventV1.EventId)),

                (Rule: IsInvalid(listenerEvent.EventAddressId),
                Parameter: nameof(ListenerEventV1.EventAddressId)),

                (Rule: IsInvalid(listenerEvent.EventListenerId),
                Parameter: nameof(ListenerEventV1.EventListenerId)),

                (Rule: IsInvalid(listenerEvent.Status),
                Parameter: nameof(ListenerEventV1.Status)),

                (Rule: IsInvalid(listenerEvent.CreatedDate),
                Parameter: nameof(ListenerEventV1.CreatedDate)),

                (Rule: IsInvalid(listenerEvent.UpdatedDate),
                Parameter: nameof(ListenerEventV1.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: listenerEvent.CreatedDate,
                    secondDate: listenerEvent.UpdatedDate,
                    secondDateName: nameof(ListenerEventV1.UpdatedDate)),

                Parameter: nameof(ListenerEventV1.CreatedDate)),

                (Rule: await IsNotRecentAsync(listenerEvent.CreatedDate),
                Parameter: nameof(ListenerEventV1.CreatedDate)));
        }

        private async ValueTask ValidateListenerEventOnModifyAsync(ListenerEventV1 listenerEvent)
        {
            ValidateListenerEventIsNotNull(listenerEvent);

            Validate(
                (Rule: IsInvalid(listenerEvent.Id),
                Parameter: nameof(ListenerEventV1.Id)),

                (Rule: IsInvalid(listenerEvent.Response),
                Parameter: nameof(ListenerEventV1.Response)),

                (Rule: IsInvalid(listenerEvent.EventId),
                Parameter: nameof(ListenerEventV1.EventId)),

                (Rule: IsInvalid(listenerEvent.EventAddressId),
                Parameter: nameof(ListenerEventV1.EventAddressId)),

                (Rule: IsInvalid(listenerEvent.EventListenerId),
                Parameter: nameof(ListenerEventV1.EventListenerId)),

                (Rule: IsInvalid(listenerEvent.Status),
                Parameter: nameof(ListenerEventV1.Status)),

                (Rule: IsInvalid(listenerEvent.CreatedDate),
                Parameter: nameof(ListenerEventV1.CreatedDate)),

                (Rule: IsInvalid(listenerEvent.UpdatedDate),
                Parameter: nameof(ListenerEventV1.UpdatedDate)),

                (Rule: IsSameAs(
                    firstDate: listenerEvent.UpdatedDate,
                    secondDate: listenerEvent.CreatedDate,
                    secondDateName: nameof(ListenerEventV1.CreatedDate)),

                Parameter: nameof(ListenerEventV1.UpdatedDate)),

                (Rule: await IsNotRecentAsync(listenerEvent.UpdatedDate),
                Parameter: nameof(ListenerEventV1.UpdatedDate)));
        }

        private static void ValidateListenerEventAgainstStorage(
            ListenerEventV1 incomingListenerEventV1,
            ListenerEventV1 storageListenerEventV1)
        {
            ValidateListenerEventExists(
                listenerEventV1: storageListenerEventV1,
                listenerEventV1Id: incomingListenerEventV1.Id);

            Validate(
                (Rule: IsNotSameAsStorage(
                    firstDate: incomingListenerEventV1.CreatedDate,
                    secondDate: storageListenerEventV1.CreatedDate),

                Parameter: nameof(ListenerEventV1.CreatedDate)),

                (Rule: IsEarlierThan(
                    firstDate: incomingListenerEventV1.UpdatedDate,
                    secondDate: storageListenerEventV1.UpdatedDate),

                Parameter: nameof(ListenerEventV1.UpdatedDate)));
        }

        private static void ValidateListenerEventId(Guid listenerEventV1Id)
        {
            Validate(
                (Rule: IsInvalid(listenerEventV1Id),
                Parameter: nameof(ListenerEventV1.Id)));
        }

        private static void ValidateListenerEventExists(ListenerEventV1 listenerEventV1, Guid listenerEventV1Id)
        {
            if (listenerEventV1 is null)
            {
                throw new NotFoundListenerEventV1Exception(
                    message: $"Could not find listener event with id: {listenerEventV1Id}.");
            }
        }

        private static void ValidateListenerEventIsNotNull(ListenerEventV1 listenerEventV1)
        {
            if (listenerEventV1 is null)
            {
                throw new NullListenerEventV1Exception(
                    message: "Listener event is null.");
            }
        }

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
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSameAsStorage(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as storage"
            };

        private static dynamic IsEarlierThan(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate) => new
            {
                Condition = firstDate < secondDate,
                Message = $"Date is earlier than storage"
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
            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV1Exception.ThrowIfContainsErrors();
        }
    }
}
