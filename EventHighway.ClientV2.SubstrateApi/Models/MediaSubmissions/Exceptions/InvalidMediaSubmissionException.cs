// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions.Exceptions
{
    public class InvalidMediaSubmissionException : Xeption
    {
        public InvalidMediaSubmissionException(string message)
            : base(message)
        { }
    }
}
