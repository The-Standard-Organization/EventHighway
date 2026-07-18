// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions
{
    public class NullEventListenerV2QueryOrchestrationException : Xeption
    {
        public NullEventListenerV2QueryOrchestrationException(string message)
            : base(message)
        { }
    }
}
