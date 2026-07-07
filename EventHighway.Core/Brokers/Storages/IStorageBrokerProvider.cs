using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    public interface IStorageBrokerProvider
    {
        void Configure(DbContextOptionsBuilder optionsBuilder);
        void ConfigureModel(ModelBuilder modelBuilder);
    }
}
