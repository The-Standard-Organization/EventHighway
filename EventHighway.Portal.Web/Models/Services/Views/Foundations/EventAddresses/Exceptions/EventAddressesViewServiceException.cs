// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses.Exceptions
{
    public class EventAddressesViewServiceException : Xeption
    {
        public EventAddressesViewServiceException(Xeption innerException)
            : base(
                message: "Event addresses view service error occurred, contact support.",
                innerException)
        { }
    }
}
