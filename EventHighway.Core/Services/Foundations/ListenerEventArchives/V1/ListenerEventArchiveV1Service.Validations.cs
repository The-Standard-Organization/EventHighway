// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V1
{
    internal partial class ListenerEventArchiveV1Service
    {
        private async ValueTask ValidateListenerEventArchiveV1OnAddAsync(
            ListenerEventArchiveV1 listenerEventArchiveV1)
        {
            ValidateListenerEventArchiveV1IsNotNull(listenerEventArchiveV1);

            Validate(
                message: "Listener event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(listenerEventArchiveV1.Id),
                Parameter: nameof(ListenerEventArchiveV1.Id)),

                (Rule: IsInvalid(listenerEventArchiveV1.EventId),
                Parameter: nameof(ListenerEventArchiveV1.EventId)),

                (Rule: IsInvalid(listenerEventArchiveV1.EventAddressId),
                Parameter: nameof(ListenerEventArchiveV1.EventAddressId)),

                (Rule: IsInvalid(listenerEventArchiveV1.EventListenerId),
                Parameter: nameof(ListenerEventArchiveV1.EventListenerId)),

                (Rule: IsInvalid(listenerEventArchiveV1.Status),
                Parameter: nameof(ListenerEventArchiveV1.Status)),

                (Rule: IsInvalid(listenerEventArchiveV1.CreatedDate),
                Parameter: nameof(ListenerEventArchiveV1.CreatedDate)),

                (Rule: IsInvalid(listenerEventArchiveV1.UpdatedDate),
                Parameter: nameof(ListenerEventArchiveV1.UpdatedDate)),

                (Rule: IsInvalid(listenerEventArchiveV1.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV1.ArchivedDate)),

                (Rule: await IsNotRecentAsync(listenerEventArchiveV1.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV1.ArchivedDate)));
        }

        private static void ValidateListenerEventArchiveV1IsNotNull(
            ListenerEventArchiveV1 listenerEventArchiveV1)
        {
            if (listenerEventArchiveV1 is null)
            {
                throw new NullListenerEventArchiveV1Exception(
                    message: "Listener event archive is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
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
            var invalidListenerEventArchiveV1Exception = new InvalidListenerEventArchiveV1Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventArchiveV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventArchiveV1Exception.ThrowIfContainsErrors();
        }
    }
}
