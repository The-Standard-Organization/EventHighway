// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V2
{
    public class EventListenerV2
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HandlerName { get; set; }

        [NotMapped]
        public Dictionary<string, string> HandlerParams { get; set; } = new();

        public string HandlerParamsJson { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public Guid EventAddressId { get; set; }
        public EventAddressV2 EventAddress { get; set; }

        public IEnumerable<ListenerEventV2> ListenerEvents { get; set; }
    }
}
