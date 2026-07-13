// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners
{
    public class EventListenerView
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HandlerName { get; set; } = string.Empty;
        public Guid HandlerId { get; set; }
        public Guid EventAddressV2Id { get; set; }
        public Guid? EventParticipantV2Id { get; set; }
        public string PromotedProperties { get; set; } = string.Empty;
        public string FilterCriteria { get; set; } = string.Empty;
    }
}
