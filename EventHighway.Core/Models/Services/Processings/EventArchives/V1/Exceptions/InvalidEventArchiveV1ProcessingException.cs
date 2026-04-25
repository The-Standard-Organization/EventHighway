// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class InvalidEventArchiveV1ProcessingException : Xeption
    {
        public InvalidEventArchiveV1ProcessingException(string message)
            : base(message)
        { }
    }
}
