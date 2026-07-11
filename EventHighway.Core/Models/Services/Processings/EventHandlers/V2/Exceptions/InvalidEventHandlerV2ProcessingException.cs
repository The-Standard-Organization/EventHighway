// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions
{
    public class InvalidEventHandlerV2ProcessingException : Xeption
    {
        public InvalidEventHandlerV2ProcessingException(string message)
            : base(message)
        { }
    }
}
