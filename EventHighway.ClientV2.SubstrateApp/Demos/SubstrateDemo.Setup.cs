// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.Seed;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApp.Demos
{
    internal sealed partial class SubstrateDemo
    {
        private async Task SetupParticipantsAndSecretsAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            this.nflix =
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
                    EventParticipantV2Id = this.nflix.Id,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            this.mediaService =
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
                    EventParticipantV2Id = this.mediaService.Id,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            this.bingeBox =
                await this.eventSubstrateBroker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Id = SeedIdentifiers.BingeBoxParticipant,
                        Name = "BingeBox",
                        Description = "BingeBox a NFlix affiliate",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            this.joe =
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

            this.ann =
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

            this.flakyBox =
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

        private async Task SetupEventAddressesAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            this.newReleases =
                await this.eventSubstrateBroker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = SeedIdentifiers.NFlixNewReleasesAddress,
                        Name = "NFlix-NewReleases",
                        Description = "NFlix New Releases",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            this.externalContributions =
                await this.eventSubstrateBroker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = SeedIdentifiers.NFlixExternalContributionsAddress,
                        Name = "NFlix-ExternalContributions",
                        Description = "Public intake for externally contributed media items.",
                        CreatedDate = now,
                        UpdatedDate = now
                    });
        }

        private async Task SetupEventListenersAsync()
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
                    EventAddressV2Id = this.externalContributions.Id,
                    EventParticipantV2Id = this.mediaService.Id,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventSubstrateBroker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.BingeBoxNewReleasesListener,
                    Name = "BingeBox New Releases Listener",
                    Description = "Receives every NFlix new release.",
                    HandlerId = this.mediaEventHandlers.BingeBox.Id,
                    HandlerName = this.mediaEventHandlers.BingeBox.Name,
                    EventAddressV2Id = this.newReleases.Id,
                    EventParticipantV2Id = this.bingeBox.Id,
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
                    EventAddressV2Id = this.newReleases.Id,
                    EventParticipantV2Id = this.joe.Id,
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
                    EventAddressV2Id = this.newReleases.Id,
                    EventParticipantV2Id = this.ann.Id,
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
                    EventAddressV2Id = this.newReleases.Id,
                    EventParticipantV2Id = this.flakyBox.Id,
                    CreatedDate = now,
                    UpdatedDate = now
                });
        }
    }
}
