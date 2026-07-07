// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.HealthArchivedEvents.V2.Exceptions
{
    internal class HealthArchivedEventsV2OrchestrationServiceException : Xeption
    {
        public HealthArchivedEventsV2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
