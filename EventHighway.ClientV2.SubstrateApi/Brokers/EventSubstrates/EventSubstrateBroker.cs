// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApi.Infrastructure;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Configurations;
using EventHighway.SqlServer;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker : IEventSubstrateBroker
    {
        private readonly EventHighwayClient eventHighwayClient;
        private readonly DatabaseGate databaseGate;

        public EventSubstrateBroker(
            string connectionString,
            EventHighwayConfiguration configuration,
            DatabaseGate databaseGate)
        {
            this.eventHighwayClient =
                new EventHighwayClient(
                    new SqlServerStorageBrokerProvider(connectionString),
                    configuration);

            this.databaseGate = databaseGate;
        }
    }
}
