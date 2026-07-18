// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions
{
    public class NullEventHandlerV2QueryProcessingException : Xeption
    {
        public NullEventHandlerV2QueryProcessingException(string message)
            : base(message)
        { }
    }
}
