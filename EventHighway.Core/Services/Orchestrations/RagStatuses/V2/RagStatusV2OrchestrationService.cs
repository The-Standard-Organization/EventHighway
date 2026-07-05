// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.RagStatuses.V2
{
    internal partial class RagStatusV2OrchestrationService : IRagStatusV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public RagStatusV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            IEventArchiveV2Service eventArchiveV2Service,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allAddresses =
                (await this.eventAddressV2Service
                    .RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                        .ToList();

            var allEvents =
                (await this.eventV2Service
                    .RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                        .ToList();

            var allArchivedEvents =
                (await this.eventArchiveV2Service
                    .RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken))
                        .ToList();

            var allListeners = allAddresses
                .SelectMany(address =>
                    address.EventListenerV2s ?? Enumerable.Empty<EventListenerV2>())
                .ToList();

            var allListenerEvents = allEvents
                .SelectMany(eventV2 =>
                    eventV2.ListenerEventV2s ?? Enumerable.Empty<ListenerEventV2>())
                .ToList();

            var allArchivedListenerEvents = allArchivedEvents
                .SelectMany(archive =>
                    archive.ListenerEventArchiveV2s ?? Enumerable.Empty<ListenerEventArchiveV2>())
                .ToList();

            int totalAddresses = allAddresses.Count;
            int totalListeners = allListeners.Count;
            int totalEvents = allEvents.Count;
            int activeEvents = allEvents.Count(e => e.Status == EventStatusV2.Active);
            int immediateEvents = allEvents.Count(e =>
                e.Status == EventStatusV2.Active && e.Type == EventTypeV2.Immediate);
            int scheduledEvents = allEvents.Count(e =>
                e.Status == EventStatusV2.Active && e.Type == EventTypeV2.Scheduled);
            int totalListenerEvents = allListenerEvents.Count;
            int pendingListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Pending);
            int successListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Success);
            int errorListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error);
            int replayListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Replay);

            decimal errorRate = totalListenerEvents > 0
                ? (decimal)errorListenerEvents / totalListenerEvents * 100
                : 0;

            decimal replayRate = totalListenerEvents > 0
                ? (decimal)replayListenerEvents / totalListenerEvents * 100
                : 0;

            int totalArchivedEvents = allArchivedEvents.Count;
            int totalArchivedListenerEvents = allArchivedListenerEvents.Count;

            int archivedListenerErrors =
                allArchivedListenerEvents.Count(la =>
                    la.Status == ListenerEventArchiveStatusV2.Error);

            decimal archiveErrorRate = totalArchivedListenerEvents > 0
                ? (decimal)archivedListenerErrors / totalArchivedListenerEvents * 100
                : 0;

            int handlerCount = allListeners
                .Select(listener => listener.HandlerId)
                .Distinct()
                .Count();

            int quarantinedEvents = allEvents.Count(e => e.Status == EventStatusV2.Quarantined);

            int quarantinedArchives =
                allArchivedEvents.Count(a => a.Status == EventArchiveStatusV2.Quarantined);

            HealthConfiguration healthConfig =
                this.configurationBroker.GetHealthConfiguration();

            HealthStatusV2 errorRateStatus =
                ComputeRagStatus(errorRate, HealthMetric.ErrorRate, healthConfig);

            HealthStatusV2 handlerStatus =
                ComputeRagStatus(handlerCount, HealthMetric.HandlerCount, healthConfig);

            HealthStatusV2 loopsDetectedStatus =
                ComputeRagStatus(quarantinedEvents, HealthMetric.LoopsDetected, healthConfig);

            HealthStatusV2 pendingBacklogStatus =
                ComputeRagStatus(pendingListenerEvents, HealthMetric.PendingBacklog, healthConfig);

            HealthStatusV2 replayRateStatus =
                ComputeRagStatus(replayRate, HealthMetric.ReplayRate, healthConfig);

            HealthStatusV2 archiveErrorRateStatus =
                ComputeRagStatus(archiveErrorRate, HealthMetric.ArchiveErrorRate, healthConfig);

            return new List<HealthCheckItemV2>
            {
                CreateItem("Event Addresses / Event Listeners / Handlers", "Total Addresses", totalAddresses.ToString(), HealthStatusV2.NA),
                CreateItem("Event Addresses / Event Listeners / Handlers", "Total Listeners", totalListeners.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Total Events", totalEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Active Events", activeEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Immediate", immediateEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Scheduled", scheduledEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Total", totalListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Pending", pendingListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Successful", successListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Errors", errorListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Error Rate %", $"{errorRate:F2}", errorRateStatus),
                CreateItem("Event Archives", "Total Archived Events", totalArchivedEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Event Archives", "Total Archived Listener Events", totalArchivedListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Event Archives", "Archived Listener Errors", archivedListenerErrors.ToString(), HealthStatusV2.NA),
                CreateItem("Event Addresses / Event Listeners / Handlers", "Registered Handlers", handlerCount.ToString(), handlerStatus),
                CreateItem("Loop Detection", "Quarantined Events", quarantinedEvents.ToString(), loopsDetectedStatus),
                CreateItem("Loop Detection", "Quarantined Archives", quarantinedArchives.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Pending Listener Events", pendingListenerEvents.ToString(), pendingBacklogStatus),
                CreateItem("Listener Events", "Replay Rate %", $"{replayRate:F2}", replayRateStatus),
                CreateItem("Event Archives", "Archive Error Rate %", $"{archiveErrorRate:F2}", archiveErrorRateStatus),
            };
        });

        private static HealthStatusV2 ComputeRagStatus(
            decimal value,
            HealthMetric metric,
            HealthConfiguration healthConfig)
        {
            RagThreshold threshold =
                healthConfig.Thresholds.FirstOrDefault(t => t.Metric == metric);

            if (threshold is null)
                return HealthStatusV2.NA;

            if (threshold.Green < threshold.Red)
            {
                if (value <= threshold.Green) return HealthStatusV2.Green;
                if (value >= threshold.Red) return HealthStatusV2.Red;
                return HealthStatusV2.Amber;
            }

            if (threshold.Green > threshold.Red)
            {
                if (value >= threshold.Green) return HealthStatusV2.Green;
                if (value <= threshold.Red) return HealthStatusV2.Red;
                return HealthStatusV2.Amber;
            }

            return HealthStatusV2.NA;
        }

        private static HealthCheckItemV2 CreateItem(
            string grouping,
            string item,
            string value,
            HealthStatusV2 status)
        {
            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value,
                StatusCode = (int)status,
                Status = status.ToString()
            };
        }
    }
}
