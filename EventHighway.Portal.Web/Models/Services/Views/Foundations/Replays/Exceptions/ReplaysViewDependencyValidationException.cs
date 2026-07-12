// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays.Exceptions
{
    public class ReplaysViewDependencyValidationException : Xeption
    {
        public ReplaysViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Replays view dependency validation error occurred, fix the errors and try again.",
                innerException)
        { }
    }
}
