// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions
{
    internal class HealthInfrastructureV2OrchestrationDependencyValidationException : Xeption
    {
        public HealthInfrastructureV2OrchestrationDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
