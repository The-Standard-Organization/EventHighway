// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using EventHighway.Core.Services.Coordinations.Events.V2;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using EventHighway.Core.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Services.Processings.EventArchives.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEventArchives.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.Core.Clients.EventHighways.V2
{
    /// <summary>
    /// Represents the V2 client implementation for the EventHighway system, providing access to
    /// various V2 event management operations and supporting event handler registration.
    /// </summary>
    internal class ClientV2 : IClientV2
    {
        private readonly string dataConnectionString;
        private readonly EventHighwayConfiguration configuration;
        private readonly EventHandlerBroker eventHandlerBroker;
        private IEventHandlerV2Service eventHandlerV2Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientV2"/> class with the specified
        /// data connection string and configuration.
        /// </summary>
        /// <param name="dataConnectionString">The connection string for the data storage.</param>
        /// <param name="configuration">The EventHighway configuration. If null, a default
        /// configuration will be created.</param>
        /// <exception cref="ArgumentException">Thrown when dataConnectionString is null or
        /// empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when required services cannot be
        /// configured or database cannot be initialized.</exception>
        public ClientV2(string dataConnectionString, EventHighwayConfiguration configuration)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dataConnectionString);
            this.dataConnectionString = dataConnectionString;
            this.configuration = configuration ?? new EventHighwayConfiguration();
            this.eventHandlerBroker = new EventHandlerBroker();
            IServiceProvider serviceProvider = ConfigureDependencies();
            InitializeClients(serviceProvider);
        }

        /// <summary>
        /// Registers an event handler with the V2 client. This method supports method chaining
        /// by returning the current instance.
        /// </summary>
        /// <param name="eventHandler">The event handler to register.</param>
        /// <returns>The current <see cref="IClientV2"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when eventHandler is null.</exception>
        public IClientV2 RegisterEventHandler(IEventHandler eventHandler)
        {
            this.eventHandlerV2Service.RegisterEventHandlerV2(eventHandler);
            return this;
        }

        /// <summary>
        /// Gets the client for managing archived events in V2 API.
        /// </summary>
        public IArchivingEventV2Client ArchivingEventV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing event addresses in V2 API.
        /// </summary>
        public IEventAddressV2Client EventAddressV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing event listeners in V2 API.
        /// </summary>
        public IEventListenerV2Client EventListenerV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing events in V2 API.
        /// </summary>
        public IEventV2Client EventV2Client { get; private set; }

        /// <summary>
        /// Gets the client for performing health checks in V2 API.
        /// </summary>
        public IHealthV2Client HealthV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing listener events in V2 API.
        /// </summary>
        public IListenerEventV2Client ListenerEventV2Client { get; private set; }

        private void InitializeClients(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var storageBroker = (StorageBroker)scope.ServiceProvider.GetRequiredService<IStorageBroker>();
                storageBroker.Database.Migrate();
            }

            this.eventHandlerV2Service =
                serviceProvider.GetRequiredService<IEventHandlerV2Service>();

            this.ArchivingEventV2Client =
                serviceProvider.GetRequiredService<IArchivingEventV2Client>();

            this.EventAddressV2Client =
                serviceProvider.GetRequiredService<IEventAddressV2Client>();

            this.EventListenerV2Client =
                serviceProvider.GetRequiredService<IEventListenerV2Client>();

            this.EventV2Client =
                serviceProvider.GetRequiredService<IEventV2Client>();

            this.HealthV2Client =
                serviceProvider.GetRequiredService<IHealthV2Client>();

            this.ListenerEventV2Client =
                serviceProvider.GetRequiredService<IListenerEventV2Client>();
        }

        private IServiceProvider ConfigureDependencies()
        {
            var serviceCollection =
                new ServiceCollection();

            RegisterBrokers(serviceCollection);
            RegisterFoundationServices(serviceCollection);
            RegisterProcessingServices(serviceCollection);
            RegisterOrchestrationServices(serviceCollection);
            RegisterCoordinationServices(serviceCollection);
            RegisterClients(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterBrokers(IServiceCollection services)
        {
            services.AddLogging();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IJsonBroker, JsonBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();

            services.AddTransient<
                IStorageBroker,
                StorageBroker>(_ =>
                    new StorageBroker(this.dataConnectionString));

            services.AddSingleton<IEventHandlerBroker>(this.eventHandlerBroker);

            services.AddSingleton<IConfigurationBroker>(
                _ => new ConfigurationBroker(this.configuration));
        }

        private static void RegisterFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IEventHandlerV2Service, EventHandlerV2Service>();
            services.AddTransient<IEventV2Service, EventV2Service>();
            services.AddTransient<IEventAddressV2Service, EventAddressV2Service>();
            services.AddTransient<IEventListenerV2Service, EventListenerV2Service>();
            services.AddTransient<IListenerEventV2Service, ListenerEventV2Service>();
            services.AddTransient<IEventCallV2Service, EventCallV2Service>();
            services.AddTransient<IEventArchiveV2Service, EventArchiveV2Service>();
            services.AddTransient<IListenerEventArchiveV2Service, ListenerEventArchiveV2Service>();
        }

        private static void RegisterProcessingServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventV2ProcessingService,
                EventV2ProcessingService>();

            services.AddTransient<
                IEventAddressV2ProcessingService,
                EventAddressV2ProcessingService>();

            services.AddTransient<
                IEventListenerV2ProcessingService,
                EventListenerV2ProcessingService>();

            services.AddTransient<
                IListenerEventV2ProcessingService,
                ListenerEventV2ProcessingService>();

            services.AddTransient<
                IEventCallV2ProcessingService,
                EventCallV2ProcessingService>();

            services.AddTransient<
                IEventArchiveV2ProcessingService,
                EventArchiveV2ProcessingService>();

            services.AddTransient<
                IListenerEventArchiveV2ProcessingService,
                ListenerEventArchiveV2ProcessingService>();
        }

        private static void RegisterOrchestrationServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventV2OrchestrationService,
                EventV2OrchestrationService>();

            services.AddTransient<
                IEventListenerV2OrchestrationService,
                EventListenerV2OrchestrationService>();

            services.AddTransient<
                IArchivingEventV2OrchestrationService,
                ArchivingEventV2OrchestrationService>();

            services.AddTransient<
                IEventArchiveV2OrchestrationService,
                EventArchiveV2OrchestrationService>();
        }

        private static void RegisterCoordinationServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventV2CoordinationService,
                EventV2CoordinationService>();

            services.AddTransient<
                IArchivingEventV2CoordinationService,
                ArchivingEventV2CoordinationService>();

            services.AddTransient<
                IHealthV2CoordinationService,
                HealthV2CoordinationService>();
        }

        private static void RegisterClients(IServiceCollection services)
        {
            services.AddTransient<
                IEventAddressV2Client,
                EventAddressV2Client>();

            services.AddTransient<
                IEventListenerV2Client,
                EventListenerV2Client>();

            services.AddTransient<
                IEventV2Client,
                EventV2Client>();

            services.AddTransient<
                IArchivingEventV2Client,
                ArchivingEventV2Client>();

            services.AddTransient<
                IHealthV2Client,
                HealthV2Client>();

            services.AddTransient<
                IListenerEventV2Client,
                ListenerEventV2Client>();
        }
    }
}
