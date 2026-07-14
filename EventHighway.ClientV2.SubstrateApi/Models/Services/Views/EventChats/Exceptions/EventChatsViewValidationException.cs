// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats.Exceptions
{
    public class EventChatsViewValidationException : Xeption
    {
        public EventChatsViewValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
