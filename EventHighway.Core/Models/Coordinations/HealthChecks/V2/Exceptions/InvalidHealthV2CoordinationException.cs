// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions
{
    internal class InvalidHealthV2CoordinationException : Xeption
    {
        public InvalidHealthV2CoordinationException(string message)
            : base(message)
        { }
    }
}
