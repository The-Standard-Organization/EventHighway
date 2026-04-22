// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2
{
    public class ListenerEventV2
    {
        public Guid Id { get; set; }
        public ListenerEventStatusV2 Status { get; set; }
        public string Response { get; set; }
        public string ResponseReasonPhrase { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public Guid EventId { get; set; }
        public EventV2 Event { get; set; }

        public Guid EventAddressId { get; set; }
        public EventAddressV2 EventAddress { get; set; }

        public Guid EventListenerId { get; set; }
        public EventListenerV2 EventListener { get; set; }
    }
}
