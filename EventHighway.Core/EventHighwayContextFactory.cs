// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using Microsoft.EntityFrameworkCore.Design;

namespace EventHighway.Core
{
    internal class ReIdentificationContextFactory : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString = String.Concat(
                "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;",
                "Trusted_Connection=True;MultipleActiveResultSets=true");

            return new StorageBroker(connectionString);
        }
    }
}
