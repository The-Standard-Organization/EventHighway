// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays
{
    public class ReplayRequestView
    {
        public Guid? EventAddressV2Id { get; set; }
        public List<Guid> EventListenerV2Ids { get; set; } = new();
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
