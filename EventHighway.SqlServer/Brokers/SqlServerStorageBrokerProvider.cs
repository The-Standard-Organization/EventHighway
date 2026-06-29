using System.Reflection.Emit;
// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Storages;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.SqlServer.Brokers
{
    public sealed class SqlServerStorageBrokerProvider : IStorageBrokerProvider
    {
        private readonly string connectionString;

        public SqlServerStorageBrokerProvider(string connectionString) =>
            this.connectionString = connectionString;

        public void Configure(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(
                this.connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly("EventHighway.SqlServer"));

        public void ConfigureModel(ModelBuilder modelBuilder) { }
    }
}