// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service
    {
        private static void ValidateEventHandlerV2OnAdd(IEventHandler eventHandler)
        {
            ValidateEventHandlerIsNotNull(eventHandler);

            Validate(
                message: "Event handler is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventHandler.Id),
                Parameter: nameof(IEventHandler.Id),
                Message: "Id required"),

                (Rule: IsInvalid(eventHandler.Name),
                Parameter: nameof(IEventHandler.Name),
                Message: "Text required"));
        }

        private static void ValidateEventHandlerV2OnRegister(IEventHandler eventHandler)
        {
            ValidateEventHandlerIsNotNull(eventHandler);

            Validate(
                message: "Event handler is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventHandler.Id),
                Parameter: nameof(IEventHandler.Id),
                Message: "Id required"),

                (Rule: IsInvalid(eventHandler.Name),
                Parameter: nameof(IEventHandler.Name),
                Message: "Text required"));
        }

        private static void ValidateEventHandlerV2Id(Guid eventHandlerV2Id)
        {
            Validate(
                message: "Event handler is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventHandlerV2Id),
                Parameter: nameof(IEventHandler.Id),
                Message: "Id required"));
        }

        private static void ValidateEventHandlerV2Exists(
            IEventHandler eventHandler,
            Guid eventHandlerV2Id)
        {
            if (eventHandler is null)
            {
                throw new NotFoundEventHandlerV2Exception(
                    message: $"Could not find event handler with id: {eventHandlerV2Id}.");
            }
        }

        private static void ValidateStorageEventHandlerV2Exists(
            EventHandlerV2 eventHandlerV2,
            Guid eventHandlerV2Id)
        {
            if (eventHandlerV2 is null)
            {
                throw new NotFoundEventHandlerV2Exception(
                    message: $"Could not find event handler with id: {eventHandlerV2Id}.");
            }
        }

        private static void ValidateEventHandlerIsNotNull(IEventHandler eventHandler)
        {
            if (eventHandler is null)
            {
                throw new NullEventHandlerV2Exception(
                    message: "Event handler is null.");
            }
        }

        private static bool IsInvalid(System.Guid id) => id == System.Guid.Empty;
        private static bool IsInvalid(string text) => string.IsNullOrWhiteSpace(text);

        private static void Validate(
            string message,
            params (bool Rule, string Parameter, string Message)[] validations)
        {
            var invalidEventHandlerV2Exception =
                new InvalidEventHandlerV2Exception(message);

            foreach ((bool rule, string parameter, string errorMessage) in validations)
            {
                if (rule)
                {
                    invalidEventHandlerV2Exception.UpsertDataList(
                        key: parameter,
                        value: errorMessage);
                }
            }

            invalidEventHandlerV2Exception.ThrowIfContainsErrors();
        }
    }
}
