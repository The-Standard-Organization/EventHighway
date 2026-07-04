// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions
{
    internal class RetryingListenerEventV2OrchestrationValidationException : Xeption
    {
        public RetryingListenerEventV2OrchestrationValidationException(
            string message,
            Xeption innerException)
            : base(message, innerException)
        { }
    }
}
