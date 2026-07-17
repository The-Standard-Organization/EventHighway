// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service
    {
        private async ValueTask ValidateEventParticipantSecretV2OnAddAsync(
            EventParticipantSecretV2 eventParticipantSecretV2)
        {
            ValidateEventParticipantSecretV2IsNotNull(eventParticipantSecretV2);

            Validate(
                message: "Event participant secret is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventParticipantSecretV2.Id),
                Parameter: nameof(EventParticipantSecretV2.Id)),

                (Rule: IsInvalid(eventParticipantSecretV2.Secret),
                Parameter: nameof(EventParticipantSecretV2.Secret)),

                (Rule: IsInvalid(eventParticipantSecretV2.CreatedDate),
                Parameter: nameof(EventParticipantSecretV2.CreatedDate)),

                (Rule: IsInvalid(eventParticipantSecretV2.UpdatedDate),
                Parameter: nameof(EventParticipantSecretV2.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventParticipantSecretV2.CreatedDate,
                    secondDate: eventParticipantSecretV2.UpdatedDate,
                    secondDateName: nameof(EventParticipantSecretV2.UpdatedDate)),
                Parameter: nameof(EventParticipantSecretV2.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventParticipantSecretV2.CreatedDate),
                Parameter: nameof(EventParticipantSecretV2.CreatedDate)),

                (Rule: IsInvalid(eventParticipantSecretV2.EventParticipantV2Id),
                Parameter: nameof(EventParticipantSecretV2.EventParticipantV2Id)),

                (Rule: IsInvalidIfSet(eventParticipantSecretV2.ActiveFrom),
                Parameter: nameof(EventParticipantSecretV2.ActiveFrom)),

                (Rule: IsInvalidIfSet(eventParticipantSecretV2.ActiveTo),
                Parameter: nameof(EventParticipantSecretV2.ActiveTo)),

                (Rule: IsRangeInvalid(
                    eventParticipantSecretV2.ActiveFrom,
                    eventParticipantSecretV2.ActiveTo),
                Parameter: nameof(EventParticipantSecretV2.ActiveTo)));
        }

        private async ValueTask ValidateEventParticipantSecretV2OnModifyAsync(
            EventParticipantSecretV2 eventParticipantSecretV2)
        {
            ValidateEventParticipantSecretV2IsNotNull(eventParticipantSecretV2);

            Validate(
                message: "Event participant secret is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventParticipantSecretV2.Id),
                Parameter: nameof(EventParticipantSecretV2.Id)),

                (Rule: IsInvalid(eventParticipantSecretV2.Secret),
                Parameter: nameof(EventParticipantSecretV2.Secret)),

                (Rule: IsInvalid(eventParticipantSecretV2.CreatedDate),
                Parameter: nameof(EventParticipantSecretV2.CreatedDate)),

                (Rule: IsInvalid(eventParticipantSecretV2.UpdatedDate),
                Parameter: nameof(EventParticipantSecretV2.UpdatedDate)),

                (Rule: IsSameAs(
                    firstDate: eventParticipantSecretV2.UpdatedDate,
                    secondDate: eventParticipantSecretV2.CreatedDate,
                    secondDateName: nameof(EventParticipantSecretV2.CreatedDate)),
                Parameter: nameof(EventParticipantSecretV2.UpdatedDate)),

                (Rule: await IsNotRecentAsync(eventParticipantSecretV2.UpdatedDate),
                Parameter: nameof(EventParticipantSecretV2.UpdatedDate)),

                (Rule: IsInvalid(eventParticipantSecretV2.EventParticipantV2Id),
                Parameter: nameof(EventParticipantSecretV2.EventParticipantV2Id)),

                (Rule: IsInvalidIfSet(eventParticipantSecretV2.ActiveFrom),
                Parameter: nameof(EventParticipantSecretV2.ActiveFrom)),

                (Rule: IsInvalidIfSet(eventParticipantSecretV2.ActiveTo),
                Parameter: nameof(EventParticipantSecretV2.ActiveTo)),

                (Rule: IsRangeInvalid(
                    eventParticipantSecretV2.ActiveFrom,
                    eventParticipantSecretV2.ActiveTo),
                Parameter: nameof(EventParticipantSecretV2.ActiveTo)));
        }

        private static void ValidateEventParticipantSecretV2AgainstStorage(
            EventParticipantSecretV2 incomingEventParticipantSecretV2,
            EventParticipantSecretV2 storageEventParticipantSecretV2)
        {
            ValidateEventParticipantSecretV2Exists(
                storageEventParticipantSecretV2,
                incomingEventParticipantSecretV2.Id);

            Validate(
                message: "Event participant secret is invalid, fix the errors and try again.",

                (Rule: IsNotSameAsStorage(
                    firstSecret: incomingEventParticipantSecretV2.Secret,
                    secondSecret: storageEventParticipantSecretV2.Secret),
                Parameter: nameof(EventParticipantSecretV2.Secret)),

                (Rule: IsNotSameAsStorage(
                    firstDate: incomingEventParticipantSecretV2.CreatedDate,
                    secondDate: storageEventParticipantSecretV2.CreatedDate),
                Parameter: nameof(EventParticipantSecretV2.CreatedDate)),

                (Rule: IsEarlierThan(
                    firstDate: incomingEventParticipantSecretV2.UpdatedDate,
                    secondDate: storageEventParticipantSecretV2.UpdatedDate),
                Parameter: nameof(EventParticipantSecretV2.UpdatedDate)));
        }

        private static void ValidateEventParticipantSecretV2Id(Guid eventParticipantSecretV2Id)
        {
            Validate(
                message: "Event participant secret is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventParticipantSecretV2Id),
                Parameter: nameof(EventParticipantSecretV2.Id)));
        }

        private static void ValidateEventParticipantSecretV2Exists(
            EventParticipantSecretV2 eventParticipantSecretV2,
            Guid eventParticipantSecretV2Id)
        {
            if (eventParticipantSecretV2 is null)
            {
                throw new NotFoundEventParticipantSecretV2Exception(
                    message: $"Could not find event participant secret with id: {eventParticipantSecretV2Id}.");
            }
        }

        private static void ValidateEventParticipantSecretV2IsNotNull(
            EventParticipantSecretV2 eventParticipantSecretV2)
        {
            if (eventParticipantSecretV2 is null)
            {
                throw new NullEventParticipantSecretV2Exception(
                    message: "Event participant secret is null.");
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

        private static dynamic IsNotSameAsStorage(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate) => new
            {
                Condition = firstDate != secondDate,
                Message = "Date is not the same as storage."
            };

        private static dynamic IsNotSameAsStorage(
            string firstSecret,
            string secondSecret) => new
            {
                Condition = firstSecret != secondSecret,
                Message = "Secret is not the same as storage."
            };

        private static dynamic IsEarlierThan(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate) => new
            {
                Condition = firstDate < secondDate,
                Message = "Date is earlier than storage."
            };

        private static dynamic IsInvalidIfSet(DateTimeOffset? date) => new
        {
            Condition = date.HasValue && date.Value == default,
            Message = "Required"
        };

        private static dynamic IsRangeInvalid(
            DateTimeOffset? activeFrom,
            DateTimeOffset? activeTo) => new
            {
                Condition = activeFrom.HasValue && activeTo.HasValue && activeTo.Value <= activeFrom.Value,
                Message = $"Date must be after {nameof(EventParticipantSecretV2.ActiveFrom)}"
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

        private static void Validate(
            string message,
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventParticipantSecretV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventParticipantSecretV2Exception.ThrowIfContainsErrors();
        }
    }
}
