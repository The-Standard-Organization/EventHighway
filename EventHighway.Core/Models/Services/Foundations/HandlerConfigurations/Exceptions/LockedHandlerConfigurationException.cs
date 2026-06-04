// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions
{
    public class LockedHandlerConfigurationException : Xeption
    {
        public LockedHandlerConfigurationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
