// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class NotFoundEventArchiveV1Exception : Xeption
    {
        public NotFoundEventArchiveV1Exception(string message)
            : base(message)
        { }
    }
}
