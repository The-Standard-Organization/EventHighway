// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class ListenerEventArchiveV1ProcessingValidationException : Xeption
    {
        public ListenerEventArchiveV1ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
