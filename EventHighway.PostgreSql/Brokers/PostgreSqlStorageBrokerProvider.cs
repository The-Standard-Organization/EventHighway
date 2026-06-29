// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Storages;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.PostgreSql.Brokers
{
    public sealed class PostgreSqlStorageBrokerProvider : IStorageBrokerProvider
    {
        private readonly string connectionString;

        public PostgreSqlStorageBrokerProvider(string connectionString) =>
            this.connectionString = connectionString;

        public void Configure(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(
                this.connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("EventHighway.PostgreSql"));

        public void ConfigureModel(ModelBuilder modelBuilder) { }
    }
}
