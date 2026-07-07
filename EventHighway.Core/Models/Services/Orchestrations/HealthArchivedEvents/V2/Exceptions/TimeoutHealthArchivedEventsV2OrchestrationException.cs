// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.HealthArchivedEvents.V2.Exceptions
{
    public class TimeoutHealthArchivedEventsV2OrchestrationException : Xeption
    {
        public TimeoutHealthArchivedEventsV2OrchestrationException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
