// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.Storages;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.SqlServer
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

        public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }

        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            // SQL Server requires no provider-specific model configuration.
        }
    }
}
