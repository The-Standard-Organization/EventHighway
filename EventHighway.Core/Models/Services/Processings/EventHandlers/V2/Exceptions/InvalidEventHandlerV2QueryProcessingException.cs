// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions
{
    public class InvalidEventHandlerV2QueryProcessingException : Xeption
    {
        public InvalidEventHandlerV2QueryProcessingException(string message)
            : base(message)
        { }
    }
}
