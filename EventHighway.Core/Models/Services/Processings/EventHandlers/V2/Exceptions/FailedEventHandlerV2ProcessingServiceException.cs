// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions
{
    public class FailedEventHandlerV2ProcessingServiceException : Xeption
    {
        public FailedEventHandlerV2ProcessingServiceException(
            string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
