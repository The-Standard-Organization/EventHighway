// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions
{
    internal class RetryingListenerEventV2OrchestrationDependencyValidationException : Xeption
    {
        public RetryingListenerEventV2OrchestrationDependencyValidationException(
            string message,
            Xeption innerException)
            : base(message, innerException)
        { }
    }
}
