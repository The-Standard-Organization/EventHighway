// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays.Exceptions
{
    public class FailedReplaysViewServiceException : Xeption
    {
        public FailedReplaysViewServiceException(Exception innerException)
            : base(
                message: "Failed replays view service error occurred, contact support.",
                innerException)
        { }
    }
}
