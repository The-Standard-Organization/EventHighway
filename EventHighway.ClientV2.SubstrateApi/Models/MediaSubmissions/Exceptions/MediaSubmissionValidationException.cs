// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions.Exceptions
{
    public class MediaSubmissionValidationException : Xeption
    {
        public MediaSubmissionValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
