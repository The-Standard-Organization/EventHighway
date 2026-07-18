// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions
{
    public class NullListenerEventV2QueryOrchestrationException : Xeption
    {
        public NullListenerEventV2QueryOrchestrationException(string message)
            : base(message)
        { }
    }
}
