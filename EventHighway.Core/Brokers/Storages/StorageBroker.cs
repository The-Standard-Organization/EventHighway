// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        private readonly string connectionString;

        public StorageBroker(string connectionString)
        {
            this.connectionString = connectionString;
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(this.connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEvents(modelBuilder.Entity<Event>());
            ConfigureEventV1s(modelBuilder.Entity<EventV1>());
            ConfigureEventV2s(modelBuilder.Entity<EventV2>());
            ConfigureEventListeners(modelBuilder.Entity<EventListener>());
            ConfigureEventListenerV1s(modelBuilder.Entity<EventListenerV1>());
            ConfigureEventListenerV2s(modelBuilder.Entity<EventListenerV2>());
            ConfigureListenerEvents(modelBuilder.Entity<ListenerEvent>());
            ConfigureListenerEventV1s(modelBuilder.Entity<ListenerEventV1>());
            ConfigureListenerEventV2s(modelBuilder.Entity<ListenerEventV2>());
            ConfigureEventV1Archives(modelBuilder.Entity<EventV1Archive>());
            ConfigureListenerEventV1Archives(modelBuilder.Entity<ListenerEventV1Archive>());
            ConfigureEventArchiveV2s(modelBuilder.Entity<EventArchiveV2>());
            ConfigureListenerEventV2Archives(modelBuilder.Entity<ListenerEventArchiveV2>());
            ConfigureHandlerConfigurations(modelBuilder.Entity<HandlerConfiguration>());
        }

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
}