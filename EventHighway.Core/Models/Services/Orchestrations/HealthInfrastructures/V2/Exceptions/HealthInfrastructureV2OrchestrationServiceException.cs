// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions
{
    internal class HealthInfrastructureV2OrchestrationServiceException : Xeption
    {
        public HealthInfrastructureV2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
