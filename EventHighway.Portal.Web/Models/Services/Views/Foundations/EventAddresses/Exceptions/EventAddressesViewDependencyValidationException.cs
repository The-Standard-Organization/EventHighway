// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses.Exceptions
{
    public class EventAddressesViewDependencyValidationException : Xeption
    {
        public EventAddressesViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Event addresses view dependency validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
