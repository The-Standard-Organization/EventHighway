// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventAddresses.V1
{
    internal partial class EventAddressV1Service
    {
        private async ValueTask ValidateEventAddressOnAddAsync(EventAddressV1 eventAddress)
        {
            ValidateEventAddressIsNotNull(eventAddress);

            Validate(
                (Rule: IsInvalid(eventAddress.Id),
                Parameter: nameof(EventAddressV1.Id)),

                (Rule: IsInvalid(eventAddress.Name),
                Parameter: nameof(EventAddressV1.Name)),

                (Rule: IsInvalid(eventAddress.Description),
                Parameter: nameof(EventAddressV1.Description)),

                (Rule: IsInvalid(eventAddress.CreatedDate),
                Parameter: nameof(EventAddressV1.CreatedDate)),

                (Rule: IsInvalid(eventAddress.UpdatedDate),
                Parameter: nameof(EventAddressV1.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventAddress.CreatedDate,
                    secondDate: eventAddress.UpdatedDate,
                    secondDateName: nameof(EventAddressV1.UpdatedDate)),

                Parameter: nameof(EventAddressV1.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventAddress.CreatedDate),
                Parameter: nameof(EventAddressV1.CreatedDate)));
        }

        private static void ValidateEventAddressId(Guid eventAddressId)
        {
            Validate(
                (Rule: IsInvalid(eventAddressId),
                Parameter: nameof(EventAddressV1.Id)));
        }

        private static void ValidateEventAddressIsNotNull(EventAddressV1 eventAddress)
        {
            if (eventAddress is null)
            {
                throw new NullEventAddressV1Exception(
                    message: "Event address is null.");
            }
        }

        private static void ValidateEventAddressExists(
            EventAddressV1 eventAddressV1,
            Guid eventAddressV1Id)
        {
            if (eventAddressV1 is null)
            {
                throw new NotFoundEventAddressV1Exception(

                    message: $"Could not find event address " +
                        $"with id: {eventAddressV1Id}.");
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
            var invalidEventAddressV1Exception =
                new InvalidEventAddressV1Exception(
                    message: "Event address is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventAddressV1Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventAddressV1Exception.ThrowIfContainsErrors();
        }
    }
}
