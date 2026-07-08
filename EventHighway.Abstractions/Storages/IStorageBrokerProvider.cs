// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace EventHighway.Abstractions.Storages
{
    public interface IStorageBrokerProvider
    {
        void Configure(DbContextOptionsBuilder optionsBuilder);
        void ConfigureModel(ModelBuilder modelBuilder);
    }
}
