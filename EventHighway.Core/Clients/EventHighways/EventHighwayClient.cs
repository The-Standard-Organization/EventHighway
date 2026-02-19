// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Apis;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Clients.EventAddresses;
using EventHighway.Core.Clients.EventAddresses.V1;
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
using EventHighway.Core.Services.Foundations.Events;
using EventHighway.Core.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;
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
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.Core.Clients.EventHighways
{
    public class EventHighwayClient : IEventHighwayClient
    {
        private readonly string dataConnectionString;

        public EventHighwayClient(string dataConnectionString)
        {
            this.dataConnectionString = dataConnectionString;
            IServiceProvider serviceProvider = ConfigureDependencies();
            InitializeClients(serviceProvider);
        }

        public IEventAddressesClient EventAddresses { get; set; }
        public IEventListenersClient EventListeners { get; set; }
        public IEventsClient Events { get; set; }
        public IEventV1sClient EventV1s { get; set; }
        public IEventAddressesV1Client EventAddressV1s { get; set; }
        public IEventListenerV1sClient EventListenerV1s { get; set; }
        public IListenerEventV1sClient ListenerEventV1s { get; set; }

        private void InitializeClients(IServiceProvider serviceProvider)
        {
            this.EventAddresses =
                serviceProvider.GetRequiredService<IEventAddressesClient>();

            this.EventListeners =
                serviceProvider.GetRequiredService<IEventListenersClient>();

            this.Events =
                serviceProvider.GetRequiredService<IEventsClient>();

            this.EventV1s =
                serviceProvider.GetRequiredService<IEventV1sClient>();

            this.EventAddressV1s =
                serviceProvider.GetRequiredService<IEventAddressesV1Client>();

            this.EventListenerV1s =
                serviceProvider.GetRequiredService<IEventListenerV1sClient>();

            this.ListenerEventV1s =
                serviceProvider.GetRequiredService<IListenerEventV1sClient>();
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
            services.AddDbContext<StorageBroker>();

            services.AddTransient<
                IStorageBroker,
                StorageBroker>(broker =>
                    new StorageBroker(this.dataConnectionString));

            services.AddTransient<IApiBroker, ApiBroker>();
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
            services.AddTransient<IEventV1ArchiveService, EventV1ArchiveService>();
            services.AddTransient<IListenerEventV1ArchiveService, ListenerEventV1ArchiveService>();
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
                IEventV1ArchiveOrchestrationService,
                EventV1ArchiveOrchestrationService>();
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

            services.AddTransient<
                IEventHighwayClient,
                EventHighwayClient>();
        }
    }
}