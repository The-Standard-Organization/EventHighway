// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Events.Exceptions
{
    public class EventsViewDependencyValidationException : Xeption
    {
        public EventsViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Events view dependency validation error occurred, fix the errors and try again.",
                innerException)
        { }
    }
}
