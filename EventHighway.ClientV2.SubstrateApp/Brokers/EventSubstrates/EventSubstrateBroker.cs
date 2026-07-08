// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Configurations;
using EventHighway.SqlServer;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker : IEventSubstrateBroker
    {
        private readonly EventHighwayClient eventHighwayClient;

        public EventSubstrateBroker(
            string connectionString,
            EventHighwayConfiguration configuration) =>
            this.eventHighwayClient =
                new EventHighwayClient(
                    new SqlServerStorageBrokerProvider(connectionString),
                    configuration);
    }
}
