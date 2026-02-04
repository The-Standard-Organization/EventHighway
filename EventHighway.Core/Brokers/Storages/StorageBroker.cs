// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        private readonly string connectionString;

        public StorageBroker(string connectionString)
        {
            this.connectionString = connectionString;

            if (!IsDesignTime())
            {
                this.Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(this.connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEvents(modelBuilder);
            ConfigureEventV1s(modelBuilder);
            ConfigureEventListeners(modelBuilder);
            ConfigureEventListenerV1s(modelBuilder);
            ConfigureListenerEvents(modelBuilder);
            ConfigureListenerEventV1s(modelBuilder);
            ConfigureListenerEventV1Archives(modelBuilder);
            ConfigureEventV1Archives(modelBuilder);
        }

        private static bool IsDesignTime() =>
            AppDomain.CurrentDomain
                .GetAssemblies()
                .Any(a => a.FullName.Contains("Microsoft.EntityFrameworkCore.Design"));

        private async ValueTask<T> InsertAsync<T>(T @object)
        {
            var broker = new StorageBroker(this.connectionString);
            broker.Entry(@object).State = EntityState.Added;
            await broker.SaveChangesAsync();

            return @object;
        }

        private async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class
        {
            var broker = new StorageBroker(this.connectionString);
            return await broker.FindAsync<T>(objectIds);
        }

        private IQueryable<T> SelectAll<T>() where T : class
        {
            var broker = new StorageBroker(this.connectionString);
            return broker.Set<T>();
        }

        private async ValueTask<T> UpdateAsync<T>(T @object)
        {
            var broker = new StorageBroker(this.connectionString);
            broker.Entry(@object).State = EntityState.Modified;
            await broker.SaveChangesAsync();

            return @object;
        }

        private async ValueTask<T> DeleteAsync<T>(T @object)
        {
            var broker = new StorageBroker(this.connectionString);
            broker.Entry(@object).State = EntityState.Deleted;
            await broker.SaveChangesAsync();

            return @object;
        }
    }

    internal sealed class StorageBrokerFactory
        : IDesignTimeDbContextFactory<StorageBroker>
    {
        public StorageBroker CreateDbContext(string[] args)
        {
            string connectionString =
    "Server=(localdb)\\MSSQLLocalDB;Database=EventHighway;Trusted_Connection=True;";


            return new StorageBroker(connectionString);
        }
    }
}
