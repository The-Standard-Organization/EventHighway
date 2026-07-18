// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Abstractions.Storages;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.PostgreSql;
using EventHighway.SqlServer;
using Microsoft.Extensions.Configuration;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public ClientBroker()
        {
            IConfiguration configuration =
              new ConfigurationBuilder()
                  .SetBasePath(AppContext.BaseDirectory)
                  .AddJsonFile("appsettings.json", optional: false)
                  .AddEnvironmentVariables()
                  .Build();

            string provider = configuration["PROVIDER"];
            string connectionString = configuration["CONNECTION_STRING"];

            IStorageBrokerProvider storageBrokerProvider =
                provider switch
            {
                "postgres" => new PostgreSqlStorageBrokerProvider(connectionString),
                _ => new SqlServerStorageBrokerProvider(connectionString)
            };

            this.eventHighwayClient = new EventHighwayClient(storageBrokerProvider);
        }

        public ClientBroker RegisterEventHandler(IEventHandler eventHandler)
        {
            this.eventHighwayClient.V2.RegisterEventHandler(eventHandler);
            return this;
        }

        public async ValueTask RegisterEventHandlerAsync(IEventHandler eventHandler) =>
            await this.eventHighwayClient.V2.RegisterEventHandlerAsync(eventHandler);
    }
}
