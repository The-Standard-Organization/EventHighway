// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2
{
    public class ListenerEventArchiveV2
    {
        public Guid Id { get; set; }
        public ListenerEventArchiveStatusV2 Status { get; set; }
        public string Response { get; set; }
        public string ResponseReasonPhrase { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }

        public Guid EventId { get; set; }
        public Guid EventAddressId { get; set; }
        public Guid EventListenerId { get; set; }
    }
}
