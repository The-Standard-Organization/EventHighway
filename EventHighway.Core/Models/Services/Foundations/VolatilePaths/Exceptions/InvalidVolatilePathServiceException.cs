// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions
{
    internal class InvalidVolatilePathServiceException : Xeption
    {
        public InvalidVolatilePathServiceException(string message)
            : base(message)
        { }
    }
}
