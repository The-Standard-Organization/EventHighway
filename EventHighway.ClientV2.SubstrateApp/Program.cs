// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApp.Demos;
using EventHighway.ClientV2.SubstrateApp.Infrastructure;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.ClientV2.SubstrateApp
{
    public static class Program
    {
        private static async Task Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSubstrateApp();

            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            serviceProvider.UseSubstrateSubscriptions();

            SubstrateDemo substrateDemo =
                serviceProvider.GetRequiredService<SubstrateDemo>();

            await substrateDemo.SetupEventAddressesEventListenersAndParticipantsAsync();
            await substrateDemo.ResetTheMediaCataloguesAsync();


            await substrateDemo.CreateMediaItemViaExternalServiceAsync(new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Yellowstone",
                Type = "Series",
                Genres = new List<string> { "Drama", "Western" },
                Rating = 8.6
            },
            SeedIdentifiers.NFlixParticipant,
            SeedIdentifiers.NFlixSecretValue);

            await substrateDemo.CreateMediaItemViaExternalServiceAsync(new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Spider-Man: Across the Spider-Verse",
                Type = "Movie",
                Genres = new List<string> { "Animation", "Action" },
                Rating = 8.5
            },
            SeedIdentifiers.NFlixParticipant,
            SeedIdentifiers.NFlixSecretValue);

            await substrateDemo.CreateMediaItemViaExternalServiceAsync(new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Guardians of the Galaxy Vol. 3",
                Type = "Movie",
                Genres = new List<string> { "Action", "Comedy" },
                Rating = 7.9
            },
            Guid.Empty,
            string.Empty);

            await substrateDemo.CreateMediaItemViaInternalServiceAsync(new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Guardians of the Galaxy Vol. 3",
                Type = "Movie",
                Genres = new List<string> { "Action", "Comedy" },
                Rating = 7.9
            });
        }
    }
}
