// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class NotFoundListenerEventArchiveV1Exception : Xeption
    {
        public NotFoundListenerEventArchiveV1Exception(string message)
            : base(message)
        { }
    }
}
