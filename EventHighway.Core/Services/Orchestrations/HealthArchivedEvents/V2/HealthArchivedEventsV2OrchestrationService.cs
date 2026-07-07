// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2
{
    internal partial class HealthArchivedEventsV2OrchestrationService : IHealthArchivedEventsV2OrchestrationService
    {
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;
        private readonly ILoggingBroker loggingBroker;

        public HealthArchivedEventsV2OrchestrationService(
            IEventArchiveV2Service eventArchiveV2Service,
            IListenerEventArchiveV2Service listenerEventArchiveV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<HealthReportV2> RetrieveHealthReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventArchiveV2> archivedEvents =
                await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync(cancellationToken);

            IQueryable<ListenerEventArchiveV2> archivedListenerEvents =
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);

            long totalEvents = archivedEvents.LongCount();

            long totalQuarantined = archivedEvents
                .LongCount(archivedEvent => archivedEvent.Status == EventArchiveStatusV2.Quarantined);

            long totalListenerEvents = archivedListenerEvents.LongCount();

            long totalSuccess = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Success);

            long totalError = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error);

            long totalActiveRetries = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = archivedListenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,

                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Total Events",
                        value: totalEvents,
                        description: "Total number of archived events."),

                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Total Quarantined",
                        value: totalQuarantined,
                        description: "Total number of quarantined archived events."),

                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Loops Detected",
                        value: totalQuarantined,
                        description: "Number of archived events quarantined by loop detection."),

                    MapToHealthCheckItem(
                        grouping: "Archived Events",
                        item: "Duplicates Blocked",
                        value: totalQuarantined,
                        description: "Number of archived events blocked as duplicates."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Total Listener Events",
                        value: totalListenerEvents,
                        description: "Total number of archived listener events."),

                    MapToRateHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Total Success",
                        count: totalSuccess,
                        total: totalListenerEvents,
                        description: "Archived listener events that completed successfully."),

                    MapToRateHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Total Error",
                        count: totalError,
                        total: totalListenerEvents,
                        description: "Archived listener events that ended in an error state."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Active (Retries Left)",
                        value: totalActiveRetries,
                        description: "Errored archived listener events with retry attempts remaining."),

                    MapToHealthCheckItem(
                        grouping: "Archived Listeners",
                        item: "Dead (No Retries)",
                        value: totalDead,
                        description: "Errored archived listener events with no retry attempts remaining.")
                }
            };
        }

        private static HealthCheckItemV2 MapToHealthCheckItem(
            string grouping,
            string item,
            long value,
            string description)
        {
            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value.ToString(CultureInfo.InvariantCulture),
                Description = description,
                StatusCode = (int)HealthStatusV2.NA,
                Status = nameof(HealthStatusV2.NA)
            };
        }

        private static HealthCheckItemV2 MapToRateHealthCheckItem(
            string grouping,
            string item,
            long count,
            long total,
            string description)
        {
            decimal rate = total == 0
                ? 0
                : (decimal)count * 100 / total;

            string value = $"{count.ToString(CultureInfo.InvariantCulture)} " +
                $"({rate.ToString("0.00", CultureInfo.InvariantCulture)}%)";

            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value,
                Description = description,
                StatusCode = (int)HealthStatusV2.NA,
                Status = nameof(HealthStatusV2.NA)
            };
        }
    }
}
