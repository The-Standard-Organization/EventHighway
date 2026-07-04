// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions
{
    internal class RetryingListenerEventV2OrchestrationServiceException : Xeption
    {
        public RetryingListenerEventV2OrchestrationServiceException(
            string message,
            Xeption innerException)
            : base(message, innerException)
        { }
    }
}
