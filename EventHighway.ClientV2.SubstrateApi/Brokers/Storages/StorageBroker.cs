// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Infrastructure;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Clients;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Storages
{
    internal partial class StorageBroker : DbContext, IStorageBroker
    {
        private readonly string connectionString;
        private readonly IEFCoreClient efCoreClient;
        private readonly DatabaseGate databaseGate;

        public StorageBroker(string connectionString, DatabaseGate databaseGate)
        {
            this.connectionString = connectionString;
            this.databaseGate = databaseGate;
            efCoreClient = new EFCoreClient(this);
            InitializeDatabase();
        }

        // The catalogue database has no migrations; create it on first use. LocalDB creates new
        // databases with AUTO_CLOSE ON, which intermittently delays re-opening connections —
        // switch it off at creation time.
        private void InitializeDatabase()
        {
            bool databaseCreated = this.Database.EnsureCreated();

            if (databaseCreated)
                this.Database.ExecuteSqlRaw("ALTER DATABASE CURRENT SET AUTO_CLOSE OFF;");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseSqlServer(this.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AddMediaItemConfigurations(modelBuilder.Entity<MediaItem>());
        }

        private async ValueTask<T> InsertAsync<T>(T @object) where T : class =>
            await efCoreClient.InsertAsync(@object);

        private async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            await efCoreClient.SelectAllAsync<T>();

        private async ValueTask<T> SelectAsync<T>(params object[] @objectIds) where T : class =>
            await efCoreClient.SelectAsync<T>(@objectIds);

        private async ValueTask<T> UpdateAsync<T>(T @object) where T : class =>
            await efCoreClient.UpdateAsync(@object);

        private async ValueTask<T> DeleteAsync<T>(T @object) where T : class =>
            await efCoreClient.DeleteAsync(@object);

        private async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkInsertAsync(objects);

        private async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkUpdateAsync(objects);

        private async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkDeleteAsync(objects);
    }
}
