// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Brokers.Storages;
using EventHighway.ClientV2.SubstrateApp.Infrastructure;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApp.Demos
{
    /// <summary>
    /// Walks the substrate story: participants and their secrets, the
    /// "NFlix-ExternalContributions" and "NFlix-NewReleases" addresses, and the listeners with
    /// their handlers — then media items created through the external intake (credentials
    /// required and verified by the substrate) or through the internal service (no credentials
    /// required). Every persisted item is relayed as a MediaItemAdded event to the
    /// NFlix-NewReleases listeners: BingeBox, Joe, Ann and FlakyBox.
    /// </summary>
    internal sealed partial class SubstrateDemo
    {
        private readonly IEventSubstrateBroker eventSubstrateBroker;
        private readonly IStorageBroker storageBroker;
        private readonly MediaEventHandlers mediaEventHandlers;
        private readonly IMediaItemService mediaItemService;
        private readonly IExternalMediaItemService externalMediaItemService;

        private EventParticipantV2 nflix;
        private EventParticipantV2 mediaService;
        private EventParticipantV2 bingeBox;
        private EventParticipantV2 joe;
        private EventParticipantV2 ann;
        private EventParticipantV2 flakyBox;
        private EventAddressV2 newReleases;
        private EventAddressV2 externalContributions;

        public SubstrateDemo(
            IEventSubstrateBroker eventSubstrateBroker,
            IStorageBroker storageBroker,
            MediaEventHandlers mediaEventHandlers,
            IMediaItemService mediaItemService,
            IExternalMediaItemService externalMediaItemService)
        {
            this.eventSubstrateBroker = eventSubstrateBroker;
            this.storageBroker = storageBroker;
            this.mediaEventHandlers = mediaEventHandlers;
            this.mediaItemService = mediaItemService;
            this.externalMediaItemService = externalMediaItemService;
        }

        /// <summary>
        /// Setup participants and their secrets,
        /// the two event addresses,
        /// and the listeners (with their handlers) that every scenario below builds on.
        /// </summary>
        public async ValueTask SetupEventAddressesEventListenersAndParticipantsAsync()
        {
            await SetupParticipantsAndSecretsAsync();
            await SetupEventAddressesAsync();
            await SetupEventListenersAsync();
        }

        /// <summary>
        /// Clean the media catalogue so each run tells the same story. Goes through the storage
        /// broker on purpose: resetting demo state is plumbing, not a business operation, so no
        /// MediaItemDeleted events are emitted for it.
        /// </summary>
        public async ValueTask ResetTheMediaCataloguesAsync()
        {
            IQueryable<MediaItem> mediaItems =
                await this.storageBroker.SelectAllMediaItemsAsync();

            foreach (MediaItem mediaItem in mediaItems.ToList())
                await this.storageBroker.DeleteMediaItemAsync(mediaItem);
        }

        /// <summary>
        /// The public intake: submits a media item with the contributing participant's
        /// credentials, which the substrate verifies. Accepted contributions flow through
        /// ExternalMediaItemService -> [NFlix-ExternalContributions] -> MediaItemService's
        /// substrate handler (persist + MediaItemAdded) -> [NFlix-NewReleases] -> listeners.
        /// </summary>
        public async ValueTask CreateMediaItemViaExternalServiceAsync(
            MediaItem mediaItem,
            Guid participantId,
            string participantSecret)
        {
            try
            {
                await this.externalMediaItemService.AddExternalMediaItemAsync(
                    new ExternalMediaItem
                    {
                        MediaItem = mediaItem,
                        ParticipantId = participantId,
                        Secret = participantSecret
                    });

                WriteMarker("  [Success]", ConsoleColor.Green, $" accepted  {mediaItem.Title}");
            }
            catch (Exception exception)
            {
                WriteMarker(
                    "  [Fail]   ", ConsoleColor.Red,
                    $" blocked   {mediaItem.Title} - {RootMessage(exception)}");
            }
        }

        /// <summary>
        /// The internal path: adds a media item straight through MediaItemService — its
        /// validations do not require credentials. The item is persisted and relayed as
        /// MediaItemAdded onto [NFlix-NewReleases] under the service's own identity.
        /// </summary>
        public async ValueTask CreateMediaItemViaInternalServiceAsync(MediaItem mediaItem)
        {
            try
            {
                await this.mediaItemService.AddMediaItemAsync(mediaItem);

                WriteMarker("  [Success]", ConsoleColor.Green, $" accepted  {mediaItem.Title}");
            }
            catch (Exception exception)
            {
                WriteMarker(
                    "  [Fail]   ", ConsoleColor.Red,
                    $" blocked   {mediaItem.Title} - {RootMessage(exception)}");
            }
        }

        private static void WriteMarker(string marker, ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(marker);
            Console.ResetColor();
            Console.WriteLine(text);
        }

        private static string RootMessage(Exception exception)
        {
            Exception current = exception;

            while (current.InnerException is not null)
                current = current.InnerException;

            return current.Message;
        }
    }
}
