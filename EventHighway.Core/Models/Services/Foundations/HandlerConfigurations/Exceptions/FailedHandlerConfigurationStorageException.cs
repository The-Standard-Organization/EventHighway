// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions
{
    public class FailedHandlerConfigurationStorageException : Xeption
    {
        public FailedHandlerConfigurationStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
