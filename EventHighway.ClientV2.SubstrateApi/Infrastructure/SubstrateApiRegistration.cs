// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApi.Brokers.Apis;
using EventHighway.ClientV2.SubstrateApi.Brokers.Configurations;
using EventHighway.ClientV2.SubstrateApi.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApi.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApi.Brokers.ReceivedEvents;
using EventHighway.ClientV2.SubstrateApi.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApi.Brokers.Storages;
using EventHighway.ClientV2.SubstrateApi.Models.Events;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaItems;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaSubmissions;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents;
using EventHighway.ClientV2.SubstrateApi.Services.Views.EventChats;
using EventHighway.Core.Models.Configurations;
using EventHighway.EventHandlers.Delegates.JoesRestApi;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace EventHighway.ClientV2.SubstrateApi.Infrastructure
{
    public static class SubstrateApiRegistration
    {
        private const string EventHighwayConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
            "Trusted_Connection=True;MultipleActiveResultSets=true";

        private const string MediaCatalogueConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=NFlixMediaDB;" +
            "Trusted_Connection=True;MultipleActiveResultSets=true";

        public static IServiceCollection AddSubstrateApi(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Infrastructure / external dependencies
            services.AddSingleton(CreateConfiguration());
            services.AddSingleton(_ => SetupWireMock(configuration));
            services.AddSingleton<DatabaseGate>();

            // Joe's downstream, read from the "JoesRestApi" section (the WireMock stand-in).
            services.AddJoesRestApiDelegateClient(configuration);

            // The same delegate client library again, pointed at a real address this time: the
            // "SubstrateApi" section, whose url is this app's own /receive endpoint. Keyed, because
            // two live instances of one interface have to be told apart.
            services.AddKeyedSingleton<IJoesRestApiDelegateClient>(
                MediaEventHandlers.SubstrateApiDelegateClientKey,
                (_, _) => new JoesRestApiDelegateClient(
                    configuration,
                    sectionName: MediaEventHandlers.SubstrateApiDelegateClientKey));

            services.AddSingleton<MediaEventHandlers>();

            // The app's own publishing identity: internal catalogue events are emitted as the
            // MediaItemService participant (its secret is seeded by the substrate setup).
            services.AddSingleton(new EventPublisherIdentity
            {
                ParticipantId = SeedIdentifiers.MediaItemServiceParticipant,
                Secret = SeedIdentifiers.MediaItemServiceSecretValue
            });

            // Brokers
            services.AddSingleton<IDateTimeBroker, DateTimeBroker>();
            services.AddSingleton<ILoggingBroker, LoggingBroker>();
            services.AddSingleton<IJsonSerializationBroker, JsonSerializationBroker>();
            services.AddSingleton<IReceivedEventBroker, ReceivedEventBroker>();
            services.AddSingleton<IConfigurationBroker, ConfigurationBroker>();
            services.AddHttpClient<IApiBroker, ApiBroker>();

            services.AddSingleton<IStorageBroker>(provider =>
                new StorageBroker(
                    MediaCatalogueConnectionString,
                    provider.GetRequiredService<DatabaseGate>()));

            services.AddSingleton<IEventSubstrateBroker>(CreateEventSubstrateBroker);

            // Foundation services
            services.AddSingleton<IMediaItemService, MediaItemService>();
            services.AddSingleton<IExternalMediaItemService, ExternalMediaItemService>();
            services.AddSingleton<IReceivedEventService, ReceivedEventService>();
            services.AddSingleton<IMediaSubmissionService, MediaSubmissionService>();

            // View services
            services.AddSingleton<IEventChatsViewService, EventChatsViewService>();

            // Substrate setup, run at startup
            services.AddSingleton<SubstrateSetup>();

            return services;
        }

        /// <summary>
        /// Completes the wiring that cannot happen while the container is being built — the
        /// substrate handler lives on MediaItemService, which depends on the substrate broker, so
        /// registering it inside the broker factory would be a circular resolution — and then lays
        /// out the participants, addresses and listeners this app rides on.
        /// </summary>
        public static async Task<IServiceProvider> UseSubstrateAsync(this IServiceProvider serviceProvider)
        {
            IEventSubstrateBroker eventSubstrateBroker =
                serviceProvider.GetRequiredService<IEventSubstrateBroker>();

            IMediaItemService mediaItemService =
                serviceProvider.GetRequiredService<IMediaItemService>();

            eventSubstrateBroker.RegisterEventHandler(
                mediaItemService.ExternalMediaItemAddedEventHandler);

            SubstrateSetup substrateSetup =
                serviceProvider.GetRequiredService<SubstrateSetup>();

            await substrateSetup.SetupEventAddressesEventListenersAndParticipantsAsync();

            return serviceProvider;
        }

        private static IEventSubstrateBroker CreateEventSubstrateBroker(IServiceProvider provider)
        {
            EventHighwayConfiguration configuration =
                provider.GetRequiredService<EventHighwayConfiguration>();

            MediaEventHandlers handlers =
                provider.GetRequiredService<MediaEventHandlers>();

            var broker = new EventSubstrateBroker(
                EventHighwayConnectionString,
                configuration,
                provider.GetRequiredService<DatabaseGate>());

            broker
                .RegisterEventHandler(handlers.SofaBox)
                .RegisterEventHandler(handlers.Joe)
                .RegisterEventHandler(handlers.Ann)
                .RegisterEventHandler(handlers.FlakyBox)
                .RegisterEventHandler(handlers.SubstrateApi);

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

        // A stand-in for the downstream REST API that Joe and Ann forward releases to. Bound to the
        // port its own appsettings url points at, which is a different port from the one the console
        // samples bind — all three can run side by side.
        //
        // The SubstrateApi listener does NOT come here: its deliveries go to a real localhost
        // endpoint, which is the whole point of this app.
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
