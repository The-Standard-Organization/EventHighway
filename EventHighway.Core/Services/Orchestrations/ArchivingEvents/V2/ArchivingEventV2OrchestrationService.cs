// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Processings.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService : IArchivingEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<EventV2>> RetrieveBatchOfQuarantinedEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            LoopDetection loopDetection =
                this.configurationBroker.GetLoopDetectionConfiguration();

            ValidateOnRetrieveBatchOfQuarantined(batchConfiguration, loopDetection);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            DateTimeOffset cutoff = now - loopDetection.Window;

            IQueryable<EventV2> allEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            IQueryable<EventV2> quarantinedEventV2s =
                allEventV2s.Where(eventV2 =>
                    eventV2.Status == EventStatusV2.Quarantined
                    && eventV2.CreatedDate < cutoff);

            int take = batchConfiguration.BatchSizeForBulkProcessing;

            return take == 0
                ? quarantinedEventV2s.AsEnumerable()
                : quarantinedEventV2s.Take(take).AsEnumerable();
        });

        public ValueTask<IEnumerable<EventV2>> RetrieveBatchOfDeadEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            ValidateOnRetrieveBatchOfDead(batchConfiguration);

            IQueryable<EventV2> deadEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sAsync(cancellationToken);

            int take = batchConfiguration.BatchSizeForBulkProcessing;

            return take == 0
                ? deadEventV2s.AsEnumerable()
                : deadEventV2s.Take(take).AsEnumerable();
        });

        public ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2sIsNotNull(eventV2s);

            await this.eventV2ProcessingService
                .BulkRemoveEventV2sAsync(eventV2s, cancellationToken);
        });
    }
}
