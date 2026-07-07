// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EventHighway.PostgresSql.Brokers
{
    public sealed class PostgresSqlStorageBrokerProvider : IStorageBrokerProvider
    {
        private readonly string connectionString;

        public PostgresSqlStorageBrokerProvider(string connectionString) =>
            this.connectionString = connectionString;

        public void Configure(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(
                this.connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("EventHighway.PostgresSql"));
        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset) ||
                        property.ClrType == typeof(DateTimeOffset?))
                    {
                        property.SetColumnType("timestamptz");
                        property.SetPrecision(6);
                    }
                }
            }
        }
    }
}
