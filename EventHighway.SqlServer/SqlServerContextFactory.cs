using System;
// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Storages;
using EventHighway.SqlServer.Brokers;
using Microsoft.EntityFrameworkCore.Design;

namespace EventHighway.SqlServer
{
    internal class SqlServerContextFactory : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString = String.Concat(
                 "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;",
                 "Trusted_Connection=True;MultipleActiveResultSets=true");

            return new StorageBroker(new SqlServerStorageBrokerProvider(connectionString));
        }
    }
}