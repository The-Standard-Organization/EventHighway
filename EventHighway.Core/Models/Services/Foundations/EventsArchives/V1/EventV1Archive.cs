// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1
{
    public class EventV1Archive
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public EventV1ArchiveType Type { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }

        public Guid EventAddressId { get; set; }
    }
}
