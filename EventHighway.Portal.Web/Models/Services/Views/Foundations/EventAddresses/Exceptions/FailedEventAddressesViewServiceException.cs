// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses.Exceptions
{
    public class FailedEventAddressesViewServiceException : Xeption
    {
        public FailedEventAddressesViewServiceException(Exception innerException)
            : base(
                message: "Failed event addresses view service error occurred, contact support.",
                innerException)
        { }
    }
}
