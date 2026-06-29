using EventHighway.Core.Brokers.Storages;
// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.PostgreSql.Brokers;
using Microsoft.EntityFrameworkCore.Design;

namespace EventHighway.PostgreSql
{
    internal class PostgreSqlContextFactory : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString =
                "Host=localhost;Port=5432;Database=EventHighwayDB;" +
                "Username=postgres;Password=postgres";

            return new StorageBroker(new PostgreSqlStorageBrokerProvider(connectionString));
        }
    }
}