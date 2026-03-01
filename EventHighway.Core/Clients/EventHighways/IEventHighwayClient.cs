// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventAddresses;
using EventHighway.Core.Clients.EventAddresses.V1;
using EventHighway.Core.Clients.EventListeners;
using EventHighway.Core.Clients.EventListeners.V1;
using EventHighway.Core.Clients.Events;
using EventHighway.Core.Clients.Events.V1;
using EventHighway.Core.Clients.ListenerEvents.V1;

namespace EventHighway.Core.Clients.EventHighways
{
    public interface IEventHighwayClient
    {
        public IEventAddressesClient EventAddresses { get; }
        public IEventListenersClient EventListeners { get; }
        public IEventsClient Events { get; }
        public IEventV1sClient EventV1s { get; set; }
        public IEventV1sClientV1 EventV1sV1 { get; set; }
        public IEventAddressesV1Client EventAddressV1s { get; set; }
        public IEventListenerV1sClient EventListenerV1s { get; set; }
        public IListenerEventV1sClient ListenerEventV1s { get; set; }
    }
}