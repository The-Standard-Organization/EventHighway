// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions
{
    internal class FailedVolatilePathServiceException : Xeption
    {
        public FailedVolatilePathServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
