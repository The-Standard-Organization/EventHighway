// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions
{
    internal class HealthV2CoordinationDependencyValidationException : Xeption
    {
        public HealthV2CoordinationDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
