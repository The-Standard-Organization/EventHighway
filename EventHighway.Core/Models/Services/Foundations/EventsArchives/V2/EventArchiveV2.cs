// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2
{
    public class EventArchiveV2
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string EventName { get; set; }
        public EventArchiveTypeV2 Type { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }

        public Guid EventAddressId { get; set; }

        public IEnumerable<ListenerEventArchiveV2> ListenerEventArchives { get; set; }
    }
}
