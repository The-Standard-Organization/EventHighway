// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal partial class EventHandlerV2ProcessingService
    {
        private static void ValidateOnRegisterEventHandlerV2(IEventHandler eventHandler) =>
            ValidateEventHandlerV2IsNotNull(eventHandler);

        private static void ValidateEventHandlerV2IsNotNull(IEventHandler eventHandler)
        {
            if (eventHandler is null)
            {
                throw new NullEventHandlerV2ProcessingException(
                    message: "Event handler is null.");
            }
        }
    }
}
