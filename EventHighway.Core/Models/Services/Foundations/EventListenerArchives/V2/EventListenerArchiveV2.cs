// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2
{
    public class EventListenerArchiveV2
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid HandlerId { get; set; }
        public string HandlerName { get; set; }
        public string PromotedProperties { get; set; }
        public string FilterCriteria { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }

        public Guid EventListenerId { get; set; }
        public Guid EventAddressId { get; set; }
        public Guid EventArchiveV2Id { get; set; }
    }
}
