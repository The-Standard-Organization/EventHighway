// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2Service
    {
        private async ValueTask ValidateListenerEventArchiveV2OnAddAsync(
            ListenerEventArchiveV2 listenerEventArchiveV2)
        {
            ValidateListenerEventArchiveV2IsNotNull(listenerEventArchiveV2);

            Validate(
                message: "Listener event archive is invalid, fix the errors and try again.",

                (Rule: IsInvalid(listenerEventArchiveV2.Id),
                Parameter: nameof(ListenerEventArchiveV2.Id)),

                (Rule: IsInvalid(listenerEventArchiveV2.EventV2Id),
                Parameter: nameof(ListenerEventArchiveV2.EventV2Id)),

                (Rule: IsInvalid(listenerEventArchiveV2.EventAddressV2Id),
                Parameter: nameof(ListenerEventArchiveV2.EventAddressV2Id)),

                (Rule: IsInvalid(listenerEventArchiveV2.EventListenerV2Id),
                Parameter: nameof(ListenerEventArchiveV2.EventListenerV2Id)),

                (Rule: IsInvalid(listenerEventArchiveV2.Status),
                Parameter: nameof(ListenerEventArchiveV2.Status)),

                (Rule: IsInvalid(listenerEventArchiveV2.CreatedDate),
                Parameter: nameof(ListenerEventArchiveV2.CreatedDate)),

                (Rule: IsInvalid(listenerEventArchiveV2.UpdatedDate),
                Parameter: nameof(ListenerEventArchiveV2.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: listenerEventArchiveV2.CreatedDate,
                    secondDate: listenerEventArchiveV2.UpdatedDate,
                    secondDateName: nameof(ListenerEventArchiveV2.UpdatedDate)),
                Parameter: nameof(ListenerEventArchiveV2.CreatedDate)),

                (Rule: IsInvalid(listenerEventArchiveV2.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV2.ArchivedDate)),

                (Rule: await IsNotRecentAsync(listenerEventArchiveV2.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV2.ArchivedDate)));
        }

        private static void ValidateListenerEventArchiveV2IsNotNull(
            ListenerEventArchiveV2 listenerEventArchiveV2)
        {
            if (listenerEventArchiveV2 is null)
            {
                throw new NullListenerEventArchiveV2Exception(
                    message: "Listener event archive is null.");
            }
        }

        private static void ValidateListenerEventArchiveV2sIsNotNull(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            if (listenerEventArchiveV2s is null)
            {
                throw new NullListenerEventArchiveV2Exception(
                    message: "Listener event archive is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
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
            var invalidListenerEventArchiveV2Exception =
                new InvalidListenerEventArchiveV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventArchiveV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventArchiveV2Exception.ThrowIfContainsErrors();
        }

        private static void ValidateListenerEventArchiveV2Query(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query)
        {
            ValidateListenerEventArchiveV2QueryIsNotNull(listenerEventArchiveV2Query);

            ValidateQuery(
                (Rule: IsNegative(listenerEventArchiveV2Query.Skip),
                Parameter: nameof(ListenerEventArchiveV2Query.Skip)),

                (Rule: IsOutOfRange(listenerEventArchiveV2Query.Take),
                Parameter: nameof(ListenerEventArchiveV2Query.Take)),

                (Rule: IsBefore(
                    firstDate: listenerEventArchiveV2Query.CreatedTo,
                    secondDate: listenerEventArchiveV2Query.CreatedFrom,
                    secondDateName: nameof(ListenerEventArchiveV2Query.CreatedFrom)),
                Parameter: nameof(ListenerEventArchiveV2Query.CreatedTo)),

                (Rule: IsBefore(
                    firstDate: listenerEventArchiveV2Query.ArchivedTo,
                    secondDate: listenerEventArchiveV2Query.ArchivedFrom,
                    secondDateName: nameof(ListenerEventArchiveV2Query.ArchivedFrom)),
                Parameter: nameof(ListenerEventArchiveV2Query.ArchivedTo)));
        }

        private static void ValidateListenerEventArchiveV2QueryIsNotNull(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query)
        {
            if (listenerEventArchiveV2Query is null)
            {
                throw new NullListenerEventArchiveV2QueryException(
                    message: "Listener event archive query is null.");
            }
        }

        private static dynamic IsNegative(int value) => new
        {
            Condition = value < 0,
            Message = "Value must be zero or greater"
        };

        private static dynamic IsOutOfRange(int value) => new
        {
            Condition = value < 1 || value > 1000,
            Message = "Value must be between 1 and 1000"
        };

        private static dynamic IsBefore(
            DateTimeOffset? firstDate,
            DateTimeOffset? secondDate,
            string secondDateName) => new
            {
                Condition = firstDate is not null
                    && secondDate is not null
                    && firstDate < secondDate,

                Message = $"Date must be after {secondDateName}"
            };

        private static void ValidateQuery(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventArchiveV2QueryException =
                new InvalidListenerEventArchiveV2QueryException(
                    message: "Listener event archive query is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventArchiveV2QueryException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventArchiveV2QueryException.ThrowIfContainsErrors();
        }
    }
}
