// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions
{
    internal class RetryingListenerEventV2OrchestrationDependencyException : Xeption
    {
        public RetryingListenerEventV2OrchestrationDependencyException(
            string message,
            Xeption innerException)
            : base(message, innerException)
        { }
    }
}
