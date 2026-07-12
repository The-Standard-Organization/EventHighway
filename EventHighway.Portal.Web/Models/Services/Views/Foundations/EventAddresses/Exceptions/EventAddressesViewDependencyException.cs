// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses.Exceptions
{
    public class EventAddressesViewDependencyException : Xeption
    {
        public EventAddressesViewDependencyException(Xeption innerException)
            : base(
                message: "Event addresses view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
