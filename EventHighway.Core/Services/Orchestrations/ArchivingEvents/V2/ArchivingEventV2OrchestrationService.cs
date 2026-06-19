// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService : IArchivingEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync() =>
        TryCatch(async () =>
        {
            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            ValidateOnRetrieveAllDeadEventV2sWithListeners(batchConfiguration);

            return await this.eventV2ProcessingService
                .RetrieveBatchOfDeadEventV2sAsync(
                    batchConfiguration.BatchSizeForBulkProcessing);
        });

        public ValueTask BulkRemoveEventV2AndListenerEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2sIsNotNull(eventV2s);

            IEnumerable<ListenerEventV2> listenerEventV2s =
                eventV2s.SelectMany(eventV2 => eventV2.ListenerEventV2s);

            await this.listenerEventV2ProcessingService
                .BulkRemoveListenerEventV2sAsync(listenerEventV2s, cancellationToken);

            await this.eventV2ProcessingService
                .BulkRemoveEventV2sAsync(eventV2s, cancellationToken);
        });

        public ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            foreach (ListenerEventV2 listenerEventV2 in eventV2.ListenerEventV2s)
            {
                await this.listenerEventV2ProcessingService
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2.Id, cancellationToken);
            }

            await this.eventV2ProcessingService.RemoveEventV2ByIdAsync(eventV2.Id, cancellationToken);
        });
    }
}
