// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1
{
    public class EventArchiveV1
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public EventArchiveV1Type Type { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }

        public Guid EventAddressId { get; set; }

        public IEnumerable<ListenerEventArchiveV1> ListenerEventArchiveV1s { get; set; }
    }
}
