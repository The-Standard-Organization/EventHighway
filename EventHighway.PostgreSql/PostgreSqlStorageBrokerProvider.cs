// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.Storages;
using EventHighway.Core.Brokers.Storages;
using EventHighway.PostgreSql.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventHighway.PostgreSql
{
    public sealed partial class PostgreSqlStorageBrokerProvider : IStorageBrokerProvider
    {
        private readonly string connectionString;

        public PostgreSqlStorageBrokerProvider(string connectionString) =>
            this.connectionString = connectionString;

        public void Configure(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(
                this.connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("EventHighway.PostgreSql"));

        public void ConfigureModel(ModelBuilder modelBuilder) { }

        public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
            configurationBuilder.Properties<DateTimeOffset>()
                .HaveColumnType("timestamptz")
                .HavePrecision(6)
                .HaveConversion<DateTimeOffsetTruncationConverter>();
    }
}
