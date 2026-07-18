// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions
{
    public class NullEventArchiveV2QueryException : Xeption
    {
        public NullEventArchiveV2QueryException(string message)
            : base(message)
        { }
    }
}
