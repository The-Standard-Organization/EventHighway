// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net;
using EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApp.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems;
using EventHighway.Core.Models.Configurations;
using EventHighway.SqlServer.Brokers;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace EventHighway.ClientV2.SubstrateApp.Infrastructure
{
    public static class SubstrateAppRegistration
    {
        private const string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
            "Trusted_Connection=True;MultipleActiveResultSets=true";

        public static IServiceCollection AddSubstrateApp(this IServiceCollection services)
        {
            // Infrastructure / external dependencies
            services.AddSingleton(CreateConfiguration());
            services.AddSingleton(_ => SetupWireMock());
            services.AddSingleton<MediaEventHandlers>();

            // Brokers
            services.AddSingleton<IDateTimeBroker, DateTimeBroker>();
            services.AddSingleton<ILoggingBroker, LoggingBroker>();
            services.AddSingleton<IJsonSerializationBroker, JsonSerializationBroker>();
            services.AddSingleton<IEventSubstrateBroker>(CreateEventSubstrateBroker);

            // Foundation services
            services.AddSingleton<IMediaItemService, MediaItemService>();
            services.AddSingleton<IExternalMediaItemService, ExternalMediaItemService>();

            return services;
        }

        private static IEventSubstrateBroker CreateEventSubstrateBroker(IServiceProvider provider)
        {
            SqlServerStorageBrokerProvider storageProvider =
                new SqlServerStorageBrokerProvider(ConnectionString);

            EventHighwayConfiguration configuration =
                provider.GetRequiredService<EventHighwayConfiguration>();

            MediaEventHandlers handlers =
                provider.GetRequiredService<MediaEventHandlers>();

            var broker = new EventSubstrateBroker(storageProvider, configuration);

            broker
                .RegisterEventHandler(handlers.BingeBox)
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

        // A stand-in for the downstream REST API that Joe and Ann forward releases to.
        private static WireMockServer SetupWireMock()
        {
            var server = WireMockServer.Start();

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
