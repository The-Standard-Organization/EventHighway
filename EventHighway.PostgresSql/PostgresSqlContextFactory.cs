// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using EventHighway.PostgresSql.Brokers;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Hosting;

namespace EventHighway.PostgresSql
{
    internal class PostgresSqlContextFactory : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString =
                Environment.GetEnvironmentVariable("CONNECTION_STRING")
                    ?? String.Concat(
                        "Host=localhost;Port=5432;Database=EventHighwayDB;" +
                        "Username=postgres;Password=postgres");

            return new StorageBroker(new PostgresSqlStorageBrokerProvider(connectionString));
        }
    }
}