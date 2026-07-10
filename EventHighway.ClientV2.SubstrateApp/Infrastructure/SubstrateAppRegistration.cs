// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Net;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApp.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApp.Brokers.Storages;
using EventHighway.ClientV2.SubstrateApp.Demos;
using EventHighway.ClientV2.SubstrateApp.Models.Events;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems;
using EventHighway.Core.Models.Configurations;
using EventHighway.EventHandlers.Delegates.JoesRestApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace EventHighway.ClientV2.SubstrateApp.Infrastructure
{
    public static class SubstrateAppRegistration
    {
        private const string EventHighwayConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
            "Trusted_Connection=True;MultipleActiveResultSets=true";

        private const string MediaCatalogueConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=NFlixMediaDB;" +
            "Trusted_Connection=True;MultipleActiveResultSets=true";

        public static IServiceCollection AddSubstrateApp(this IServiceCollection services)
        {
            IConfiguration configuration = BuildConfiguration();

            // Infrastructure / external dependencies
            services.AddSingleton(configuration);
            services.AddSingleton(CreateConfiguration());
            services.AddSingleton(_ => SetupWireMock(configuration));
            services.AddJoesRestApiDelegateClient(configuration);
            services.AddSingleton<MediaEventHandlers>();

            // The app's own publishing identity: internal catalogue events are emitted as the
            // MediaItemService participant (its secret is seeded by the demo setup).
            services.AddSingleton(new EventPublisherIdentity
            {
                ParticipantId = SeedIdentifiers.MediaItemServiceParticipant,
                Secret = SeedIdentifiers.MediaItemServiceSecretValue
            });

            // Brokers
            services.AddSingleton<IDateTimeBroker, DateTimeBroker>();
            services.AddSingleton<ILoggingBroker, LoggingBroker>();
            services.AddSingleton<IJsonSerializationBroker, JsonSerializationBroker>();
            services.AddSingleton<IStorageBroker>(_ => new StorageBroker(MediaCatalogueConnectionString));
            services.AddSingleton<IEventSubstrateBroker>(CreateEventSubstrateBroker);

            // Foundation services
            services.AddSingleton<IMediaItemService, MediaItemService>();
            services.AddSingleton<IExternalMediaItemService, ExternalMediaItemService>();

            // Demo runner
            services.AddSingleton<SubstrateDemo>();

            return services;
        }

        // Completes the wiring that cannot happen while the container is being built: the
        // substrate handler lives on MediaItemService, which depends on the substrate broker —
        // registering it inside the broker factory below would be a circular resolution.
        public static IServiceProvider UseSubstrateSubscriptions(this IServiceProvider serviceProvider)
        {
            IEventSubstrateBroker eventSubstrateBroker =
                serviceProvider.GetRequiredService<IEventSubstrateBroker>();

            IMediaItemService mediaItemService =
                serviceProvider.GetRequiredService<IMediaItemService>();

            eventSubstrateBroker.RegisterEventHandler(mediaItemService.ExternalMediaItemAddedEventHandler);

            return serviceProvider;
        }

        private static IEventSubstrateBroker CreateEventSubstrateBroker(IServiceProvider provider)
        {
            EventHighwayConfiguration configuration =
                provider.GetRequiredService<EventHighwayConfiguration>();

            MediaEventHandlers handlers =
                provider.GetRequiredService<MediaEventHandlers>();

            var broker = new EventSubstrateBroker(EventHighwayConnectionString, configuration);

            broker
                .RegisterEventHandler(handlers.SofaBox)
                .RegisterEventHandler(handlers.Joe)
                .RegisterEventHandler(handlers.Ann)
                .RegisterEventHandler(handlers.FlakyBox);

            return broker;
        }

        // Loop detection: only allow 1 identical item per minute.
        private static EventHighwayConfiguration CreateConfiguration()
        {
            var configuration = new EventHighwayConfiguration();
            configuration.LoopDetection.Enabled = true;
            configuration.LoopDetection.Threshold = 0;
            configuration.LoopDetection.Window = TimeSpan.FromMinutes(1);

            return configuration;
        }

        private static IConfiguration BuildConfiguration() =>
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

        // A stand-in for the downstream REST API that Joe and Ann forward releases to.
        // Joe's delegate client reads its target url from appsettings, so the server is
        // bound to the port that url points at — guaranteeing the configuration and the
        // stand-in agree.
        private static WireMockServer SetupWireMock(IConfiguration configuration)
        {
            var joesApiUrl = new Uri(configuration["JoesRestApi:Url"]);
            var server = WireMockServer.Start(joesApiUrl.Port);

            server
                .Given(WireMock.RequestBuilders.Request.Create().WithPath("/token").UsingPost())
                .RespondWith(WireMock.ResponseBuilders.Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"access_token\":\"demo-token\",\"token_type\":\"Bearer\",\"expires_in\":3600}"));

            server
                .Given(WireMock.RequestBuilders.Request.Create().WithPath("/events").UsingPost())
                .RespondWith(WireMock.ResponseBuilders.Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody("Event received"));

            return server;
        }
    }
}
