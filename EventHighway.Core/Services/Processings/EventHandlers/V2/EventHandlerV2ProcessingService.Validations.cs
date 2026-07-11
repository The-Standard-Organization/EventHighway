// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal partial class EventHandlerV2ProcessingService
    {
        private static void ValidateOnRegisterEventHandlerV2(IEventHandler eventHandler) =>
            ValidateEventHandlerV2IsNotNull(eventHandler);

        private static void ValidateOnRemoveEventHandlerV2ById(Guid eventHandlerV2Id) =>
            ValidateEventHandlerV2Id(eventHandlerV2Id);

        private static void ValidateOnRetrieveOrRegisterEventHandlerV2(IEventHandler eventHandler)
        {
            ValidateEventHandlerV2IsNotNull(eventHandler);

            Validate(
                message: "Event handler is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventHandler.Id),
                Parameter: nameof(IEventHandler.Id)));
        }

        private static void ValidateEventHandlerV2IsNotNull(IEventHandler eventHandler)
        {
            if (eventHandler is null)
            {
                throw new NullEventHandlerV2ProcessingException(
                    message: "Event handler is null.");
            }
        }

        private static void ValidateEventHandlerV2Id(Guid eventHandlerV2Id)
        {
            Validate(
                message: "Event handler is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventHandlerV2Id),
                Parameter: nameof(IEventHandler.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventHandlerV2ProcessingException =
                new InvalidEventHandlerV2ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventHandlerV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventHandlerV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
