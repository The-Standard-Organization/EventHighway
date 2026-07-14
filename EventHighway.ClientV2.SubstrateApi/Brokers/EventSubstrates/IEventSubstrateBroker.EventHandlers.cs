// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        IEventSubstrateBroker RegisterEventHandler(IEventHandler eventHandler);
    }
}
