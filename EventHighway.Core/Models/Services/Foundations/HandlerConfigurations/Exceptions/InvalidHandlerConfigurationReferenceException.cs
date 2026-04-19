// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions
{
    public class InvalidHandlerConfigurationReferenceException : Xeption
    {
        public InvalidHandlerConfigurationReferenceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
