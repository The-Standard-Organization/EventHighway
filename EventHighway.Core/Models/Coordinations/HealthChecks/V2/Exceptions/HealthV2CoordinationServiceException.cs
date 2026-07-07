// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions
{
    internal class HealthV2CoordinationServiceException : Xeption
    {
        public HealthV2CoordinationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
