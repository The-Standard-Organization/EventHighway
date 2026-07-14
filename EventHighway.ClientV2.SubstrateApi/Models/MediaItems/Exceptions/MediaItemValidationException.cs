// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaItems.Exceptions
{
    public class MediaItemValidationException : Xeption
    {
        public MediaItemValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
