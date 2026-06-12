// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.Core.Clients.EventHighways.V2
{
    internal class ClientV2 : IClientV2
    {
        public ClientV2(IServiceProvider serviceProvider)
        {
            this.ArchivingEvent2Client = serviceProvider.GetRequiredService<IArchivingEvent2Client>();
            this.EventAddressV2Client = serviceProvider.GetRequiredService<IEventAddressV2Client>();
            this.EventListenerV2Client = serviceProvider.GetRequiredService<IEventListenerV2Client>();
            this.EventV2Client = serviceProvider.GetRequiredService<IEventV2Client>();
            this.ListenerEventV2Client = serviceProvider.GetRequiredService<IListenerEventV2Client>();
        }

        public IArchivingEvent2Client ArchivingEvent2Client { get; }
        public IEventAddressV2Client EventAddressV2Client { get; }
        public IEventListenerV2Client EventListenerV2Client { get; }
        public IEventV2Client EventV2Client { get; }
        public IListenerEventV2Client ListenerEventV2Client { get; }
    }
}
