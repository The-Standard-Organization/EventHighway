// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.EventHighways;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public ClientBroker()
        {
            string connectionString = String.Concat(
                "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDb;",
                "Trusted_Connection=True;MultipleActiveResultSets=true");

            this.eventHighwayClient =
                new EventHighwayClient(connectionString);
        }
    }
}
