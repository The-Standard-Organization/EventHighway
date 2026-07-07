// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Apis;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Clients.EventAddresses;
using EventHighway.Core.Clients.EventAddresses.V1;
using EventHighway.Core.Clients.EventHighways.V2;
using EventHighway.Core.Clients.EventListeners;
using EventHighway.Core.Clients.EventListeners.V1;
using EventHighway.Core.Clients.Events;
using EventHighway.Core.Clients.Events.V1;
using EventHighway.Core.Clients.ListenerEvents.V1;
using EventHighway.Core.Services.Coordinations.Events;
using EventHighway.Core.Services.Coordinations.Events.V1;
using EventHighway.Core.Services.Foundations.EventAddresses;
using EventHighway.Core.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Foundations.EventCalls;
using EventHighway.Core.Services.Foundations.EventCalls.V1;
using EventHighway.Core.Services.Foundations.EventListeners;
using EventHighway.Core.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.Events;
using EventHighway.Core.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.ListernEvents;
using EventHighway.Core.Services.Foundations.ListernEvents.V1;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Services.Orchestrations.EventListeners;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using EventHighway.Core.Services.Orchestrations.Events;
using EventHighway.Core.Services.Orchestrations.Events.V1;
using EventHighway.Core.Services.Processings.EventAddresses.V1;
using EventHighway.Core.Services.Processings.EventCalls;
using EventHighway.Core.Services.Processings.EventCalls.V1;
using EventHighway.Core.Services.Processings.EventListeners;
using EventHighway.Core.Services.Processings.EventListeners.V1;
using EventHighway.Core.Services.Processings.Events.V1;
using EventHighway.Core.Services.Processings.ListenerEvents;
using EventHighway.Core.Services.Processings.ListenerEvents.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.Core.Clients.EventHighways
{
    /// <summary>
    /// Represents the main client for the EventHighway system, providing access to various
    /// event management operations including events, listeners, addresses, and their operations
    /// across different API versions.
    /// </summary>
    public class EventHighwayClient : IEventHighwayClient
    {
        private readonly IStorageBrokerProvider storageProvider;
        private readonly EventHighwayConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHighwayClient"/> 
        /// class using the specified storage provider.
        /// </summary>
        /// <param name="storageProvider">
        /// The storage provider responsible for configuring and accessing the underlying data store.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="storageProvider"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required services cannot be configured or the client fails to initialize.
        /// </exception>
        public EventHighwayClient(IStorageBrokerProvider storageProvider)
        {
            ArgumentNullException.ThrowIfNull(storageProvider);
            this.storageProvider = storageProvider;
            this.configuration = new EventHighwayConfiguration();
            IServiceProvider serviceProvider = ConfigureDependencies();
            InitializeClients(serviceProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHighwayClient"/> class with the
        /// specified storage provider and configuration.
        /// </summary>
        /// <param name="storageProvider">
        /// The storage provider responsible for configuring and accessing the underlying data store.
        /// </param>
        /// <param name="configuration">
        /// The EventHighway configuration. If null, a default configuration will be used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="storageProvider"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required services cannot be configured or the client fails to initialize.
        /// </exception>
        public EventHighwayClient(IStorageBrokerProvider storageProvider, EventHighwayConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(storageProvider);
            this.storageProvider = storageProvider;
            this.configuration = configuration ?? new EventHighwayConfiguration();
            IServiceProvider serviceProvider = ConfigureDependencies();
            InitializeClients(serviceProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHighwayClient"/> class with the
        /// specified storage provider and event handlers.
        /// </summary>
        /// <param name="storageProvider">
        /// The storage provider responsible for configuring and accessing the underlying data store.
        /// </param>
        /// <param name="eventHandlers">
        /// The event handlers to register with the client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="storageProvider"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required services cannot be configured, event handlers cannot be registered,
        /// or the client fails to initialize.
        /// </exception>
        public EventHighwayClient(IStorageBrokerProvider storageProvider, params IEventHandler[] eventHandlers)
            : this(storageProvider)
        {
            foreach (IEventHandler eventHandler in eventHandlers)
                this.V2.RegisterEventHandler(eventHandler);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHighwayClient"/> class with the
        /// specified storage provider, configuration, and event handlers.
        /// </summary>
        /// <param name="storageProvider">
        /// The storage provider responsible for configuring and accessing the underlying data store.
        /// </param>
        /// <param name="configuration">
        /// The EventHighway configuration. If null, a default configuration will be used.
        /// </param>
        /// <param name="eventHandlers">
        /// The event handlers to register with the client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="storageProvider"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required services cannot be configured, event handlers cannot be registered,
        /// or the client fails to initialize.
        /// </exception>
        public EventHighwayClient(
            IStorageBrokerProvider storageProvider,
            EventHighwayConfiguration configuration,
            params IEventHandler[] eventHandlers)
            : this(storageProvider, configuration)
        {
            foreach (IEventHandler eventHandler in eventHandlers)
                this.V2.RegisterEventHandler(eventHandler);
        }

        /// <summary>
        /// Gets the client for managing event addresses.
        /// </summary>
        public IEventAddressesClient EventAddresses { get; private set; }

        /// <summary>
        /// Gets the client for managing event listeners.
        /// </summary>
        public IEventListenersClient EventListeners { get; private set; }

        /// <summary>
        /// Gets the client for managing events.
        /// </summary>
        public IEventsClient Events { get; private set; }

        /// <summary>
        /// Gets the client for managing events in V1 API.
        /// </summary>
        public IEventV1sClient EventV1s { get; private set; }

        /// <summary>
        /// Gets the client for managing events in V1 API with V1 operations.
        /// </summary>
        public IEventV1sClientV1 EventV1sV1 { get; private set; }

        /// <summary>
        /// Gets the client for managing event addresses in V1 API.
        /// </summary>
        public IEventAddressesV1Client EventAddressV1s { get; private set; }

        /// <summary>
        /// Gets the client for managing event listeners in V1 API.
        /// </summary>
        public IEventListenerV1sClient EventListenerV1s { get; private set; }

        /// <summary>
        /// Gets the client for managing listener events in V1 API.
        /// </summary>
        public IListenerEventV1sClient ListenerEventV1s { get; private set; }

        /// <summary>
        /// Gets the V2 API client for advanced event management and handler registration.
        /// </summary>
        public IClientV2 V2 { get; private set; }

        private void InitializeClients(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var storageBroker = (StorageBroker)scope.ServiceProvider.GetRequiredService<IStorageBroker>();
                storageBroker.Database.Migrate();
            }

            this.EventAddresses =
                serviceProvider.GetRequiredService<IEventAddressesClient>();

            this.EventListeners =
                serviceProvider.GetRequiredService<IEventListenersClient>();

            this.Events =
                serviceProvider.GetRequiredService<IEventsClient>();

            this.EventV1s =
                serviceProvider.GetRequiredService<IEventV1sClient>();

            this.EventV1sV1 =
                serviceProvider.GetRequiredService<IEventV1sClientV1>();

            this.EventAddressV1s =
                serviceProvider.GetRequiredService<IEventAddressesV1Client>();

            this.EventListenerV1s =
                serviceProvider.GetRequiredService<IEventListenerV1sClient>();

            this.ListenerEventV1s =
                serviceProvider.GetRequiredService<IListenerEventV1sClient>();

            this.V2 = new ClientV2(this.storageProvider, this.configuration);
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
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();

            services.AddTransient<
                IStorageBroker,
                StorageBroker>(_ =>
                    new StorageBroker(this.storageProvider));

            services.AddTransient<IApiBroker, ApiBroker>();

            services.AddSingleton<IConfigurationBroker>(
                _ => new ConfigurationBroker(this.configuration));
        }

        private static void RegisterFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IEventAddressService, EventAddressService>();
            services.AddTransient<IEventListenerService, EventListenerService>();
            services.AddTransient<IListenerEventService, ListenerEventService>();
            services.AddTransient<IEventCallService, EventCallService>();
            services.AddTransient<IEventV1Service, EventV1Service>();
            services.AddTransient<IEventListenerV1Service, EventListenerV1Service>();
            services.AddTransient<IListenerEventV1Service, ListenerEventV1Service>();
            services.AddTransient<IEventCallV1Service, EventCallV1Service>();
            services.AddTransient<IEventAddressV1Service, EventAddressV1Service>();
            services.AddTransient<IEventArchiveV1Service, EventArchiveV1Service>();
            services.AddTransient<IListenerEventArchiveV1Service, ListenerEventArchiveV1Service>();
            services.AddTransient<IEventListenerV2Service, EventListenerV2Service>();
            services.AddTransient<IListenerEventV2Service, ListenerEventV2Service>();
        }

        private static void RegisterProcessingServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventListenerProcessingService,
                EventListenerProcessingService>();

            services.AddTransient<
                IEventCallProcessingService,
                EventCallProcessingService>();

            services.AddTransient<
                IListenerEventProcessingService,
                ListenerEventProcessingService>();

            services.AddTransient<
                IEventCallV1ProcessingService,
                EventCallV1ProcessingService>();

            services.AddTransient<
                IEventListenerV1ProcessingService,
                EventListenerV1ProcessingService>();

            services.AddTransient<
                IEventV1ProcessingService,
                EventV1ProcessingService>();

            services.AddTransient<
                IListenerEventV1ProcessingService,
                ListenerEventV1ProcessingService>();

            services.AddTransient<
                IEventAddressV1ProcessingService,
                EventAddressV1ProcessingService>();
        }

        private static void RegisterOrchestrationServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventListenerOrchestrationService,
                EventListenerOrchestrationService>();

            services.AddTransient<
                IEventOrchestrationService,
                EventOrchestrationService>();

            services.AddTransient<
                IEventListenerV1OrchestrationService,
                EventListenerV1OrchestrationService>();

            services.AddTransient<
                IEventV1OrchestrationService,
                EventV1OrchestrationService>();

            services.AddTransient<
                IEventV1OrchestrationServiceV1,
                EventV1OrchestrationServiceV1>();

            services.AddTransient<
                IEventArchiveV1OrchestrationService,
                EventArchiveV1OrchestrationService>();
        }

        private static void RegisterCoordinationServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventCoordinationService,
                EventCoordinationService>();

            services.AddTransient<
                IEventV1CoordinationService,
                EventV1CoordinationService>();

            services.AddTransient<
                IEventV1CoordinationServiceV1,
                EventV1CoordinationServiceV1>();
        }

        private static void RegisterClients(IServiceCollection services)
        {
            services.AddTransient<
                IEventsClient,
                EventsClient>();

            services.AddTransient<
                IEventV1sClient,
                EventV1sClient>();

            services.AddTransient<
                IEventV1sClientV1,
                EventV1sClientV1>();

            services.AddTransient<
                IEventListenersClient,
                EventListenersClient>();

            services.AddTransient<
                IEventListenerV1sClient,
                EventListenerV1sClient>();

            services.AddTransient<
                IEventAddressesClient,
                EventAddressesClient>();

            services.AddTransient<
                IEventAddressesV1Client,
                EventAddressesV1Client>();

            services.AddTransient<
                IListenerEventV1sClient,
                ListenerEventV1sClient>();
        }
    }
}