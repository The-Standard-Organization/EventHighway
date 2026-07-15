// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.Storages;
using EventHighway.PostgreSql.Interceptors;
using Microsoft.EntityFrameworkCore;

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
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("EventHighway.PostgreSql"))
                .AddInterceptors(new DateTimeOffsetTruncationInterceptor());

        public void ConfigureModel(ModelBuilder modelBuilder)
        { }

        public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
            configurationBuilder.Properties<DateTimeOffset>()
                .HaveColumnType("timestamptz")
                .HavePrecision(6);
    }
}
