// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class InvalidEventArchiveV1Exception : Xeption
    {
        public InvalidEventArchiveV1Exception(string message)
            : base(message)
        { }
    }
}
