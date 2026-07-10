// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Foundations.EventPosts.Exceptions
{
    public class EventPostValidationException : Xeption
    {
        public EventPostValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
