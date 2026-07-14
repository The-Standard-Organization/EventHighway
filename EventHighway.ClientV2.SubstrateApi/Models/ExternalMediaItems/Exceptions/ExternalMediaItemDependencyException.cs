// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.ExternalMediaItems.Exceptions
{
    public class ExternalMediaItemDependencyException : Xeption
    {
        public ExternalMediaItemDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
