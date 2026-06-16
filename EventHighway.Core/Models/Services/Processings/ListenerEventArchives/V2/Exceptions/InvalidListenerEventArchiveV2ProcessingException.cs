// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions
{
    public class InvalidListenerEventArchiveV2ProcessingException : Xeption
    {
        public InvalidListenerEventArchiveV2ProcessingException(string message)
            : base(message)
        { }
    }
}
