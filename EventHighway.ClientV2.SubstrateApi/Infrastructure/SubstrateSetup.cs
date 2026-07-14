// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaItems;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApi.Infrastructure
{
    /// <summary>
    /// Lays out the same highway the SubstrateApp console sample does — the participants and their
    /// secrets, the "NFlix-ExternalContributions" and "NFlix-NewReleases" addresses, and the
    /// listeners bound to each — and adds the one this app brings with it: an unfiltered listener
    /// that relays every new release to the chat.
    /// </summary>
    /// <remarks>
    /// Every row is keyed on a shared, fixed Guid and written with a retrieve-or-add, so this runs
    /// safely against a database the console samples have already seeded, in any order, any number
    /// of times.
    /// </remarks>
    public sealed class SubstrateSetup
    {
        private readonly IEventSubstrateBroker eventSubstrateBroker;
        private readonly IMediaItemService mediaItemService;
        private readonly MediaEventHandlers mediaEventHandlers;

        public SubstrateSetup(
            IEventSubstrateBroker eventSubstrateBroker,
            IMediaItemService mediaItemService,
            MediaEventHandlers mediaEventHandlers)
        {
            this.eventSubstrateBroker = eventSubstrateBroker;
            this.mediaItemService = mediaItemService;
            this.mediaEventHandlers = mediaEventHandlers;
        }

        public async ValueTask SetupEventAddressesEventListenersAndParticipantsAsync()
        {
            await SetupParticipantsAndSecretsAsync();

            (EventAddressV2 newReleases, EventAddressV2 externalContributions) =
                await SetupEventAddressesAsync();

            await SetupEventListenersAsync(newReleases, externalContributions);
        }

        private async ValueTask SetupParticipantsAndSecretsAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.NFlixParticipant,
                    Name = "NFlix",
                    Description = "NFlix streaming platform.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantSecretAsync(
                new EventParticipantSecretV2
                {
                    Id = SeedIdentifiers.NFlixSecret,
                    Secret = SeedIdentifiers.NFlixSecretValue,
                    EventParticipantV2Id = SeedIdentifiers.NFlixParticipant,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.MediaItemServiceParticipant,
                    Name = "MediaItemService",
                    Description = "Internal service that ingests external contributions.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // MediaItemService publishes MediaItemAdded/Updated/Deleted as itself, so it needs
            // its own verified secret (see EventPublisherIdentity in the DI registration).
            await this.eventSubstrateBroker.AddParticipantSecretAsync(
                new EventParticipantSecretV2
                {
                    Id = SeedIdentifiers.MediaItemServiceSecret,
                    Secret = SeedIdentifiers.MediaItemServiceSecretValue,
                    EventParticipantV2Id = SeedIdentifiers.MediaItemServiceParticipant,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // This app's own identity — the one the chat box presents to /submit, and the one that
            // owns the unfiltered listener below. It publishes AND subscribes, which is why it is
            // the only listening participant here that needs a secret of its own.
            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.SubstrateApiParticipant,
                    Name = "SubstrateApi",
                    Description = "The SubstrateApi chat app: submits media items and shows every release.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantSecretAsync(
                new EventParticipantSecretV2
                {
                    Id = SeedIdentifiers.SubstrateApiSecret,
                    Secret = SeedIdentifiers.SubstrateApiSecretValue,
                    EventParticipantV2Id = SeedIdentifiers.SubstrateApiParticipant,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.SofaBoxParticipant,
                    Name = "SofaBox",
                    Description = "SofaBox a NFlix affiliate",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.JoeParticipant,
                    Name = "Joe",
                    Description = "Joe, a movie buff.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.AnnParticipant,
                    Name = "Ann",
                    Description = "Ann, a movie fan.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.FlakyBoxParticipant,
                    Name = "FlakyBox",
                    Description = "An affiliate whose endpoint is always down.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });
        }

        private async ValueTask<(EventAddressV2 NewReleases, EventAddressV2 ExternalContributions)>
            SetupEventAddressesAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            EventAddressV2 newReleases =
                await this.eventSubstrateBroker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = SeedIdentifiers.NFlixNewReleasesAddress,
                        Name = "NFlix-NewReleases",
                        Description = "NFlix New Releases",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventAddressV2 externalContributions =
                await this.eventSubstrateBroker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = SeedIdentifiers.NFlixExternalContributionsAddress,
                        Name = "NFlix-ExternalContributions",
                        Description = "Public intake for externally contributed media items.",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            return (newReleases, externalContributions);
        }

        private async ValueTask SetupEventListenersAsync(
            EventAddressV2 newReleases,
            EventAddressV2 externalContributions)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // The ingestion seam — MediaItemService's ONLY subscription: its substrate handler
            // (see MediaItemService.Substrate.cs) listens on NFlix-ExternalContributions and
            // funnels every accepted contribution into the media catalogue (service -> storage
            // broker + substrate broker). It receives only the event content — never the
            // publisher's credentials. MediaItemService does not listen to NFlix-NewReleases;
            // it publishes there.
            IEventHandler ingestionHandler =
                this.mediaItemService.ExternalMediaItemAddedEventHandler;

            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.MediaItemServiceContributionsListener,
                    Name = "MediaItemService Contributions Listener",
                    Description = "Ingests accepted external contributions into the media catalogue.",
                    HandlerId = ingestionHandler.Id,
                    HandlerName = ingestionHandler.Name,
                    EventAddressV2Id = externalContributions.Id,
                    EventParticipantV2Id = SeedIdentifiers.MediaItemServiceParticipant,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // No filter, no promoted properties: whatever reaches the address reaches the chat,
            // whole. Every other listener here is selective about something — that is the point of
            // this one.
            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.SubstrateApiNewReleasesListener,
                    Name = "SubstrateApi New Releases Listener",
                    Description = "Relays every new release, unfiltered, to the SubstrateApi chat UI.",
                    HandlerId = this.mediaEventHandlers.SubstrateApi.Id,
                    HandlerName = this.mediaEventHandlers.SubstrateApi.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = SeedIdentifiers.SubstrateApiParticipant,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.SofaBoxNewReleasesListener,
                    Name = "SofaBox New Releases Listener",
                    Description = "Receives every NFlix new release.",
                    HandlerId = this.mediaEventHandlers.SofaBox.Id,
                    HandlerName = this.mediaEventHandlers.SofaBox.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = SeedIdentifiers.SofaBoxParticipant,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.JoeGoodMoviesListener,
                    Name = "Joe Good Movies Listener",
                    Description = "Forwards movies rated 8.0 or higher to Joe's API.",
                    HandlerId = this.mediaEventHandlers.Joe.Id,
                    HandlerName = this.mediaEventHandlers.Joe.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = SeedIdentifiers.JoeParticipant,
                    PromotedProperties = "Title,Type,Rating",
                    FilterCriteria =
                        "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.AnnNewReleasesListener,
                    Name = "Ann New Releases Listener",
                    Description = "Forwards every NFlix new release to Ann's API.",
                    HandlerId = this.mediaEventHandlers.Ann.Id,
                    HandlerName = this.mediaEventHandlers.Ann.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = SeedIdentifiers.AnnParticipant,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.FlakyBoxNewReleasesListener,
                    Name = "FlakyBox New Releases Listener",
                    Description = "An always-unavailable affiliate; every delivery to it fails.",
                    HandlerId = this.mediaEventHandlers.FlakyBox.Id,
                    HandlerName = this.mediaEventHandlers.FlakyBox.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = SeedIdentifiers.FlakyBoxParticipant,
                    CreatedDate = now,
                    UpdatedDate = now
                });
        }
    }
}
