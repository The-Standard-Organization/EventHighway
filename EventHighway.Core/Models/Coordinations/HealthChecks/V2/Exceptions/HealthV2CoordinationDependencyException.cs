// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions
{
    internal class HealthV2CoordinationDependencyException : Xeption
    {
        public HealthV2CoordinationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
