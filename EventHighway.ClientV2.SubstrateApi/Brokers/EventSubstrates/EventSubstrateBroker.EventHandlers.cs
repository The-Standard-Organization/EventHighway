// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public IEventSubstrateBroker RegisterEventHandler(IEventHandler eventHandler)
        {
            this.eventHighwayClient.V2.RegisterEventHandler(eventHandler);

            return this;
        }
    }
}
