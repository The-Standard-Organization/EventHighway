// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
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
        public EventTypeV1 Type { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public int RetryAttempts { get; set; }

        public Guid EventAddressId { get; set; }
        public EventAddressV1 EventAddressV1 { get; set; }

        public ICollection<ListenerEventV1> ListenerEventV1s { get; set; }
    }
}
