// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.PostgresSql.Brokers;
using EventHighway.SqlServer.Brokers;
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

            string provider = configuration["Database:CurrentProvider"];

            string connectionString =
                configuration[$"Database:Providers:{provider}:ConnectionString"];

            IStorageBrokerProvider storageProvider =
                provider switch
                {
                    "postgres" =>
                        new PostgresSqlStorageBrokerProvider(connectionString),

                    _ =>
                        new SqlServerStorageBrokerProvider(connectionString)
                };

            this.eventHighwayClient = new EventHighwayClient(storageProvider);
        }

        public ClientBroker RegisterEventHandler(IEventHandler eventHandler)
        {
            this.eventHighwayClient.V2.RegisterEventHandler(eventHandler);
            return this;
        }
    }
}
