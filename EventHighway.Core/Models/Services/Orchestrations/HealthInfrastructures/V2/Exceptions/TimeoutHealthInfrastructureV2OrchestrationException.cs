// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions
{
    public class TimeoutHealthInfrastructureV2OrchestrationException : Xeption
    {
        public TimeoutHealthInfrastructureV2OrchestrationException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
