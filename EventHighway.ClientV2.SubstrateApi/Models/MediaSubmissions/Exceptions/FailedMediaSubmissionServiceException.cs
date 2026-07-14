// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions.Exceptions
{
    public class FailedMediaSubmissionServiceException : Xeption
    {
        public FailedMediaSubmissionServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
