// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal partial class HandlerConfigurationService
    {
        private static ValueTask ValidateHandlerConfigurationOnAddAsync(
            HandlerConfiguration handlerConfiguration)
        {
            ValidateHandlerConfigurationIsNotNull(handlerConfiguration);

            Validate(
                (Rule: IsInvalid(handlerConfiguration.Id),
                Parameter: nameof(HandlerConfiguration.Id)),

                (Rule: IsInvalid(handlerConfiguration.Name),
                Parameter: nameof(HandlerConfiguration.Name)),

                (Rule: IsInvalid(handlerConfiguration.Value),
                Parameter: nameof(HandlerConfiguration.Value)));

            return ValueTask.CompletedTask;
        }

        private static void ValidateHandlerConfigurationIsNotNull(
            HandlerConfiguration handlerConfiguration)
        {
            if (handlerConfiguration is null)
            {
                throw new NullHandlerConfigurationException(
                    message: "Handler configuration is null.");
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

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidHandlerConfigurationException.AddData(
                        key: parameter,
                        values: rule.Message);
                }
            }

            invalidHandlerConfigurationException.ThrowIfContainsErrors();
        }
    }
}
