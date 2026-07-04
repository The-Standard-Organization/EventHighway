// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions
{
    public class NullRetryingListenerEventV2OrchestrationException : Xeption
    {
        public NullRetryingListenerEventV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
