// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Configurations;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker : IEventSubstrateBroker
    {
        private readonly IStorageBrokerProvider storageProvider;
        private readonly EventHighwayClient eventHighwayClient;

        public EventSubstrateBroker(
            IStorageBrokerProvider storageProvider,
            EventHighwayConfiguration configuration) =>
            this.eventHighwayClient = new EventHighwayClient(storageProvider, configuration);
    }
}
