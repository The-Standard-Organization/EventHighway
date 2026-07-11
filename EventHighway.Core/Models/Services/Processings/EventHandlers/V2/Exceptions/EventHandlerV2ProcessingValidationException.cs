// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions
{
    internal class EventHandlerV2ProcessingValidationException : Xeption
    {
        public EventHandlerV2ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
