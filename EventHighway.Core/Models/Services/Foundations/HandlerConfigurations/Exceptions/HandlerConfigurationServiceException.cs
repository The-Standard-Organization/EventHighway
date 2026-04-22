// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions
{
    public class HandlerConfigurationServiceException : Xeption
    {
        public HandlerConfigurationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
