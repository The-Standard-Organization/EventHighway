// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApi.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApi.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApi.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApi.Brokers.Storages;
using EventHighway.ClientV2.SubstrateApi.Models.Events;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaItems
{
    // The internal media catalogue: a foundation CRUD service that persists media items through
    // the storage broker. Deliberate deviation from The Standard, kept small for this demo: every
    // successful add, modify and remove ALSO emits MediaItemAdded / MediaItemUpdated /
    // MediaItemDeleted onto "NFlix-NewReleases" through the substrate broker, publishing as this
    // app's own participant. A strictly compliant shape would lift that pairing of two capital
    // resources (storage + event substrate) into an orchestration:
    // exposer -> orchestration -> foundation -> broker.
    internal partial class MediaItemService : IMediaItemService
    {
        private const string MediaItemAddedEventName = "MediaItemAdded";
        private const string MediaItemUpdatedEventName = "MediaItemUpdated";
        private const string MediaItemDeletedEventName = "MediaItemDeleted";

        private readonly IStorageBroker storageBroker;
        private readonly IEventSubstrateBroker eventSubstrateBroker;
        private readonly IJsonSerializationBroker jsonSerializationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly EventPublisherIdentity eventPublisherIdentity;

        public MediaItemService(
            IStorageBroker storageBroker,
            IEventSubstrateBroker eventSubstrateBroker,
            IJsonSerializationBroker jsonSerializationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker,
            EventPublisherIdentity eventPublisherIdentity)
        {
            this.storageBroker = storageBroker;
            this.eventSubstrateBroker = eventSubstrateBroker;
            this.jsonSerializationBroker = jsonSerializationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
            this.eventPublisherIdentity = eventPublisherIdentity;
        }

        public ValueTask<MediaItem> AddMediaItemAsync(MediaItem mediaItem) =>
        TryCatch(async () =>
        {
            ValidateMediaItemOnAdd(mediaItem);

            MediaItem addedMediaItem =
                await this.storageBroker.InsertMediaItemAsync(mediaItem);

            await EmitMediaItemEventAsync(addedMediaItem, MediaItemAddedEventName);

            return addedMediaItem;
        });

        public ValueTask<IQueryable<MediaItem>> RetrieveAllMediaItemsAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllMediaItemsAsync());

        public ValueTask<MediaItem> RetrieveMediaItemByIdAsync(Guid mediaItemId) =>
        TryCatch(async () =>
        {
            ValidateMediaItemId(mediaItemId);

            MediaItem maybeMediaItem =
                await this.storageBroker.SelectMediaItemByIdAsync(mediaItemId);

            ValidateStorageMediaItemExists(maybeMediaItem, mediaItemId);

            return maybeMediaItem;
        });

        public ValueTask<MediaItem> ModifyMediaItemAsync(MediaItem mediaItem) =>
        TryCatch(async () =>
        {
            ValidateMediaItemOnModify(mediaItem);

            MediaItem maybeMediaItem =
                await this.storageBroker.SelectMediaItemByIdAsync(mediaItem.Id);

            ValidateStorageMediaItemExists(maybeMediaItem, mediaItem.Id);

            MediaItem modifiedMediaItem =
                await this.storageBroker.UpdateMediaItemAsync(mediaItem);

            await EmitMediaItemEventAsync(modifiedMediaItem, MediaItemUpdatedEventName);

            return modifiedMediaItem;
        });

        public ValueTask<MediaItem> RemoveMediaItemByIdAsync(Guid mediaItemId) =>
        TryCatch(async () =>
        {
            ValidateMediaItemId(mediaItemId);

            MediaItem maybeMediaItem =
                await this.storageBroker.SelectMediaItemByIdAsync(mediaItemId);

            ValidateStorageMediaItemExists(maybeMediaItem, mediaItemId);

            MediaItem deletedMediaItem =
                await this.storageBroker.DeleteMediaItemAsync(maybeMediaItem);

            await EmitMediaItemEventAsync(deletedMediaItem, MediaItemDeletedEventName);

            return deletedMediaItem;
        });

        private async ValueTask EmitMediaItemEventAsync(MediaItem mediaItem, string eventName)
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            await this.eventSubstrateBroker.EmitAsync(
                new EventEnvelope<MediaItem>
                {
                    EventName = eventName,
                    Content = mediaItem,
                    EventAddressId = SeedIdentifiers.NFlixNewReleasesAddress,
                    ParticipantId = this.eventPublisherIdentity.ParticipantId,
                    Secret = this.eventPublisherIdentity.Secret,
                    OccurredAt = now
                });
        }
    }
}
