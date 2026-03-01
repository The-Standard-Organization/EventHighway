// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1
{
    public class ListenerEventV1Archive
    {
        public Guid Id { get; set; }
        public ListenerEventV1ArchiveStatus Status { get; set; }
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
