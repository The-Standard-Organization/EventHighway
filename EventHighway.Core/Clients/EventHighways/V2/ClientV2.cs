// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Hashings;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventArchives.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.EventParticipantSecrets.V2;
using EventHighway.Core.Clients.EventParticipants.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Clients.ListenerEventArchives.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;
using EventHighway.Core.Clients.ReplayingEvents.V2;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using EventHighway.Core.Services.Coordinations.Events.V2;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Coordinations.ReplayingEvents.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using EventHighway.Core.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.AddressSummaries.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.DuplicateSummaries.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.EventFirings.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.EventParticipants.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;
using EventHighway.Core.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.LoopDetections.V2;
using EventHighway.Core.Services.Orchestrations.ParticipantSummaries.V2;
using EventHighway.Core.Services.Orchestrations.RagStatuses.V2;
using EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.RestoringEvents.V2;
using EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.RetrySummaries.V2;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Services.Processings.EventArchives.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEventArchives.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.Traffics.V2;
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
        /// Gets the client for retrieving archived events in V2 API.
        /// </summary>
        public IEventArchiveV2Client EventArchiveV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing event listeners in V2 API.
        /// </summary>
        public IEventListenerV2Client EventListenerV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing event participant secrets in V2 API.
        /// </summary>
        public IEventParticipantSecretV2Client EventParticipantSecretV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing event participants in V2 API.
        /// </summary>
        public IEventParticipantV2Client EventParticipantV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing events in V2 API.
        /// </summary>
        public IEventV2Client EventV2Client { get; private set; }

        /// <summary>
        /// Gets the container exposing the V2 health check sub-clients.
        /// </summary>
        public IHealthClientV2 HealthClientV2 { get; private set; }

        /// <summary>
        /// Gets the client for retrieving archived listener events in V2 API.
        /// </summary>
        public IListenerEventArchiveV2Client ListenerEventArchiveV2Client { get; private set; }

        /// <summary>
        /// Gets the client for managing listener events in V2 API.
        /// </summary>
        public IListenerEventV2Client ListenerEventV2Client { get; private set; }

        /// <summary>
        /// Gets the client for replaying archived events in V2 API.
        /// </summary>
        public IReplayingEventV2Client ReplayingEventV2Client { get; private set; }

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

            this.EventArchiveV2Client =
                serviceProvider.GetRequiredService<IEventArchiveV2Client>();

            this.EventListenerV2Client =
                serviceProvider.GetRequiredService<IEventListenerV2Client>();

            this.EventParticipantSecretV2Client =
                serviceProvider.GetRequiredService<IEventParticipantSecretV2Client>();

            this.EventParticipantV2Client =
                serviceProvider.GetRequiredService<IEventParticipantV2Client>();

            this.EventV2Client =
                serviceProvider.GetRequiredService<IEventV2Client>();

            this.HealthClientV2 =
                serviceProvider.GetRequiredService<IHealthClientV2>();

            this.ListenerEventArchiveV2Client =
                serviceProvider.GetRequiredService<IListenerEventArchiveV2Client>();

            this.ListenerEventV2Client =
                serviceProvider.GetRequiredService<IListenerEventV2Client>();

            this.ReplayingEventV2Client =
                serviceProvider.GetRequiredService<IReplayingEventV2Client>();
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
            services.AddTransient<IHashBroker, HashBroker>();

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
            services.AddTransient<IEventParticipantV2Service, EventParticipantV2Service>();
            services.AddTransient<IEventParticipantSecretV2Service, EventParticipantSecretV2Service>();
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

            services.AddTransient<
                ITrafficV2ProcessingService,
                TrafficV2ProcessingService>();
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
                IEventFiringV2OrchestrationService,
                EventFiringV2OrchestrationService>();

            services.AddTransient<
                IListenerEventV2OrchestrationService,
                ListenerEventV2OrchestrationService>();

            services.AddTransient<
                IEventParticipantV2OrchestrationService,
                EventParticipantV2OrchestrationService>();

            services.AddTransient<
                IArchivingEventV2OrchestrationService,
                ArchivingEventV2OrchestrationService>();

            services.AddTransient<
                IEventArchiveV2OrchestrationService,
                EventArchiveV2OrchestrationService>();

            services.AddTransient<
                IRestoringEventV2OrchestrationService,
                RestoringEventV2OrchestrationService>();

            services.AddTransient<
                IReplayingListenerEventV2OrchestrationService,
                ReplayingListenerEventV2OrchestrationService>();

            services.AddTransient<
                IRetryingListenerEventV2OrchestrationService,
                RetryingListenerEventV2OrchestrationService>();

            services.AddTransient<
                IRagStatusV2OrchestrationService,
                RagStatusV2OrchestrationService>();

            services.AddTransient<
                IAddressSummaryV2OrchestrationService,
                AddressSummaryV2OrchestrationService>();

            services.AddTransient<
                ILoopDetectionV2OrchestrationService,
                LoopDetectionV2OrchestrationService>();

            services.AddTransient<
                IDuplicateSummaryV2OrchestrationService,
                DuplicateSummaryV2OrchestrationService>();

            services.AddTransient<
                IRetrySummaryV2OrchestrationService,
                RetrySummaryV2OrchestrationService>();

            services.AddTransient<
                IParticipantSummaryV2OrchestrationService,
                ParticipantSummaryV2OrchestrationService>();

            services.AddTransient<
                IHealthInfrastructureV2OrchestrationService,
                HealthInfrastructureV2OrchestrationService>();

            services.AddTransient<
                IHealthEventsV2OrchestrationService,
                HealthEventsV2OrchestrationService>();

            services.AddTransient<
                IHealthArchivedEventsV2OrchestrationService,
                HealthArchivedEventsV2OrchestrationService>();
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
                IReplayingEventV2CoordinationService,
                ReplayingEventV2CoordinationService>();

            services.AddTransient<
                IHealthV2CoordinationService,
                HealthV2CoordinationService>();
        }

        private static void RegisterClients(IServiceCollection services)
        {
            services.AddTransient<
                IArchivingEventV2Client,
                ArchivingEventV2Client>();

            services.AddTransient<
                IEventAddressV2Client,
                EventAddressV2Client>();

            services.AddTransient<
                IEventArchiveV2Client,
                EventArchiveV2Client>();

            services.AddTransient<
                IEventListenerV2Client,
                EventListenerV2Client>();

            services.AddTransient<
                IEventParticipantSecretV2Client,
                EventParticipantSecretV2Client>();

            services.AddTransient<
                IEventParticipantV2Client,
                EventParticipantV2Client>();

            services.AddTransient<
                IEventV2Client,
                EventV2Client>();

            services.AddTransient<
                IHealthClientV2,
                HealthClientV2>();

            services.AddTransient<
                IHealthAddressClientV2,
                HealthAddressClientV2>();

            services.AddTransient<
                IHealthDuplicateClientV2,
                HealthDuplicateClientV2>();

            services.AddTransient<
                IHealthLoopClientV2,
                HealthLoopClientV2>();

            services.AddTransient<
                IHealthParticipantClientV2,
                HealthParticipantClientV2>();

            services.AddTransient<
                IHealthRetryClientV2,
                HealthRetryClientV2>();

            services.AddTransient<
                IHealthStatusClientV2,
                HealthStatusClientV2>();

            services.AddTransient<
                IHealthTrafficClientV2,
                HealthTrafficClientV2>();

            services.AddTransient<
                IListenerEventArchiveV2Client,
                ListenerEventArchiveV2Client>();

            services.AddTransient<
                IListenerEventV2Client,
                ListenerEventV2Client>();

            services.AddTransient<
                IReplayingEventV2Client,
                ReplayingEventV2Client>();
        }
    }
}
