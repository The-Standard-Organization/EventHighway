// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListenerV2> EventListenersV2 { get; set; }
    }
}
