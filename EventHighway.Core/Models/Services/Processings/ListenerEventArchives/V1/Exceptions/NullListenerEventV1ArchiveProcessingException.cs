// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class NullListenerEventArchiveV1ProcessingException : Xeption
    {
        public NullListenerEventArchiveV1ProcessingException(string message)
            : base(message)
        { }
    }
}
