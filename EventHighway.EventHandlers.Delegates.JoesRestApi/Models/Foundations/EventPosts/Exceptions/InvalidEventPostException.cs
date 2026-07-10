// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Foundations.EventPosts.Exceptions
{
    public class InvalidEventPostException : Xeption
    {
        public InvalidEventPostException(string message)
            : base(message)
        { }
    }
}
