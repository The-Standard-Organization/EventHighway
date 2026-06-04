// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions
{
    public class InvalidReferenceEventListenerV1Exception : Xeption
    {
        public InvalidReferenceEventListenerV1Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
