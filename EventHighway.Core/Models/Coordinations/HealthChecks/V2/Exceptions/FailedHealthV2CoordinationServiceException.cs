// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions
{
    public class FailedHealthV2CoordinationServiceException : Xeption
    {
        public FailedHealthV2CoordinationServiceException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
