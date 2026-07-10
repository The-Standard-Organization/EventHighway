// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.Storages;
using EventHighway.Core.Brokers.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventHighway.PostgreSql
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

        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            // PostgreSQL timestamptz stores 6 fractional digits and ROUNDS the 100ns
            // tick .NET carries, so a written value can read back different. Truncate
            // on write so in-memory values always round-trip unchanged.
            ValueConverter<DateTimeOffset, DateTimeOffset> truncateToMicroseconds =
                new ValueConverter<DateTimeOffset, DateTimeOffset>(
                    dateTimeOffset => dateTimeOffset.AddTicks(
                        -(dateTimeOffset.Ticks % TimeSpan.TicksPerMicrosecond)),

                    dateTimeOffset => dateTimeOffset);

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset) ||
                        property.ClrType == typeof(DateTimeOffset?))
                    {
                        property.SetColumnType("timestamptz");
                        property.SetPrecision(6);
                        property.SetValueConverter(truncateToMicroseconds);
                    }
                }
            }
        }
    }
}
