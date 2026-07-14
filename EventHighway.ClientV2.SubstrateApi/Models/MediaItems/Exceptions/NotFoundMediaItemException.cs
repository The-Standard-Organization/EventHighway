// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaItems.Exceptions
{
    public class NotFoundMediaItemException : Xeption
    {
        public NotFoundMediaItemException(string message)
            : base(message)
        { }
    }
}
