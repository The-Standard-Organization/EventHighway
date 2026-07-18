// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions
{
    public class InvalidListenerEventArchiveV2QueryException : Xeption
    {
        public InvalidListenerEventArchiveV2QueryException(string message)
            : base(message)
        { }
    }
}
