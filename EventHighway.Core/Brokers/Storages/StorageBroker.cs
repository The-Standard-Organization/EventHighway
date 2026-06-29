// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Clients;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        private readonly IStorageBrokerProvider provider;
        private readonly IEFCoreClient efCoreClient;

        public StorageBroker(IStorageBrokerProvider provider)
        {
            this.provider = provider;
            efCoreClient = new EFCoreClient(this);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            this.provider.Configure(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEventParticipantV2s(modelBuilder.Entity<EventParticipantV2>());
            ConfigureEventParticipantSecretV2s(modelBuilder.Entity<EventParticipantSecretV2>());
            ConfigureEvents(modelBuilder.Entity<Event>());
            ConfigureEventV1s(modelBuilder.Entity<EventV1>());
            ConfigureEventV2s(modelBuilder.Entity<EventV2>());
            ConfigureEventAddressV1s(modelBuilder.Entity<EventAddressV1>());
            ConfigureEventAddressV2s(modelBuilder.Entity<EventAddressV2>());
            ConfigureEventAddresses(modelBuilder.Entity<EventAddress>());
            ConfigureEventArchiveV1s(modelBuilder.Entity<EventArchiveV1>());
            ConfigureEventArchiveV2s(modelBuilder.Entity<EventArchiveV2>());
            ConfigureEventListenerV1s(modelBuilder.Entity<EventListenerV1>());
            ConfigureEventListenerV2s(modelBuilder.Entity<EventListenerV2>());
            ConfigureEventListeners(modelBuilder.Entity<EventListener>());
            ConfigureListenerEventV1s(modelBuilder.Entity<ListenerEventV1>());
            ConfigureListenerEventV2s(modelBuilder.Entity<ListenerEventV2>());
            ConfigureListenerEventArchiveV1s(modelBuilder.Entity<ListenerEventArchiveV1>());
            ConfigureListenerEventArchiveV2s(modelBuilder.Entity<ListenerEventArchiveV2>());
            ConfigureListenerEvents(modelBuilder.Entity<ListenerEvent>());

            this.provider.ConfigureModel(modelBuilder);
        }

        private async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default)
             where T : class =>
                 await efCoreClient.InsertAsync(@object, cancellationToken);

        private async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default)
            where T : class =>
                await efCoreClient.SelectAllAsync<T>(cancellationToken);

        private async ValueTask<T> SelectAsync<T>(object[] @objectIds, CancellationToken cancellationToken = default)
            where T : class =>
                await efCoreClient.SelectAsync<T>(@objectIds, cancellationToken);

        private async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
                await efCoreClient.UpdateAsync(@object, cancellationToken);

        private async ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default)
            where T : class =>
                await efCoreClient.DeleteAsync(@object, cancellationToken);

        private async ValueTask BulkInsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
                where T : class =>
                    await efCoreClient.BulkInsertAsync<T>(objects, useTransaction, cancellationToken);

        private async ValueTask<IEnumerable<T>> BulkReadAsync<T>(
            IEnumerable<T> objects,
            CancellationToken cancellationToken = default)
                where T : class =>
                    await efCoreClient.BulkReadAsync<T>(objects, cancellationToken);

        private async ValueTask BulkUpdateAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
                where T : class =>
                    await efCoreClient.BulkUpdateAsync<T>(objects, useTransaction, cancellationToken);

        private async ValueTask BulkDeleteAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
                where T : class =>
                    await efCoreClient.BulkDeleteAsync<T>(objects, useTransaction, cancellationToken);

        private async ValueTask BulkUpsertAsync<T>(
            IEnumerable<T> objects,
            bool useTransaction = true,
            CancellationToken cancellationToken = default)
                where T : class =>
                    await efCoreClient.BulkUpsertAsync<T>(objects, useTransaction, cancellationToken);

        private async ValueTask<bool> ExistsAsync<T>(
            object[] objectIds,
            CancellationToken cancellationToken = default)
                where T : class =>
                    await efCoreClient.ExistsAsync<T>(objectIds, cancellationToken);
    }
}