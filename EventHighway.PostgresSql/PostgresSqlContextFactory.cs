// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Storages;
using EventHighway.PostgresSql.Brokers;
using Microsoft.EntityFrameworkCore.Design;

namespace EventHighway.PostgresSql
{
    internal class PostgresSqlContextFactory : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString =
                "Host=localhost;Port=5432;Database=EventHighwayDB;" +
                "Username=postgres;Password=postgres";

            return new StorageBroker(new PostgresSqlStorageBrokerProvider(connectionString));
        }
    }
}