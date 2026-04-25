// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions
{
    public class InvalidEventCallV1ReferenceException : Xeption
    {
        public InvalidEventCallV1ReferenceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
