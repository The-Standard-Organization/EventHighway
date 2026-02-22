// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Models.Services.Foundations.Events.V1
{
    public class EventV1
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public EventV1Type Type { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }

        public Guid EventAddressId { get; set; }
        public EventAddressV1 EventAddress { get; set; }

        public ICollection<ListenerEventV1> ListenerEvents { get; set; }
    }
}
