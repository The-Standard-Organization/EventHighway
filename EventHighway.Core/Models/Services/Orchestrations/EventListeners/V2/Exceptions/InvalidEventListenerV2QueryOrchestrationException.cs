// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions
{
    public class InvalidEventListenerV2QueryOrchestrationException : Xeption
    {
        public InvalidEventListenerV2QueryOrchestrationException(string message)
            : base(message)
        { }
    }
}
