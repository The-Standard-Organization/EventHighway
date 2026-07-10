// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using Microsoft.EntityFrameworkCore.Design;

namespace EventHighway.PostgreSql
{
    internal class PostgreSqlContextFactory : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString =
                Environment.GetEnvironmentVariable("CONNECTION_STRING")
                    ?? String.Concat(
                        "Host=localhost;Port=5432;Database=EventHighwayDB;" +
                        "Username=postgres;Password=postgres");

            return new StorageBroker(new PostgreSqlStorageBrokerProvider(connectionString));
        }
    }
}