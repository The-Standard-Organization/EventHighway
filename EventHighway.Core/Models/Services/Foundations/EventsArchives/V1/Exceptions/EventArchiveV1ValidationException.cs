// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class EventArchiveV1ValidationException : Xeption
    {
        public EventArchiveV1ValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
