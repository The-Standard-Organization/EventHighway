// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions
{
    public class InvalidListenerEventV2QueryOrchestrationException : Xeption
    {
        public InvalidListenerEventV2QueryOrchestrationException(string message)
            : base(message)
        { }
    }
}
