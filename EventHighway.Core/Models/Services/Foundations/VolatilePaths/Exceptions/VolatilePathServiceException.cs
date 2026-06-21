// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions
{
    internal class VolatilePathServiceException : Xeption
    {
        public VolatilePathServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
