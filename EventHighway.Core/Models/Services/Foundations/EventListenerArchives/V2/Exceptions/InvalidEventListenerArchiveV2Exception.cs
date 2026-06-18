// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions
{
    public class InvalidEventListenerArchiveV2Exception : Xeption
    {
        public InvalidEventListenerArchiveV2Exception(string message)
            : base(message)
        { }
    }
}
