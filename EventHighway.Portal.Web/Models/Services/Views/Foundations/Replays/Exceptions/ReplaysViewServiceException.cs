// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays.Exceptions
{
    public class ReplaysViewServiceException : Xeption
    {
        public ReplaysViewServiceException(Xeption innerException)
            : base(
                message: "Replays view service error occurred, contact support.",
                innerException)
        { }
    }
}
