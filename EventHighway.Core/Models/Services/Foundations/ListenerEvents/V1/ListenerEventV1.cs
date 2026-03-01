// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1
{
    public class ListenerEventV1
    {
        public Guid Id { get; set; }
        public ListenerEventV1Status Status { get; set; }
        public string Response { get; set; }
        public string ResponseReasonPhrase { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public Guid EventId { get; set; }
        public EventV1 Event { get; set; }

        public Guid EventAddressId { get; set; }
        public EventAddressV1 EventAddress { get; set; }

        public Guid EventListenerId { get; set; }
        public EventListenerV1 EventListener { get; set; }
    }
}
