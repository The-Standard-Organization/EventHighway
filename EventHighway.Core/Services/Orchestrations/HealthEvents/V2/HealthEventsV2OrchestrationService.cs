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
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.HealthEvents.V2
{
    internal partial class HealthEventsV2OrchestrationService : IHealthEventsV2OrchestrationService
    {
        private readonly IEventV2Service eventV2Service;
        private readonly IListenerEventV2Service listenerEventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public HealthEventsV2OrchestrationService(
            IEventV2Service eventV2Service,
            IListenerEventV2Service listenerEventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventV2Service = eventV2Service;
            this.listenerEventV2Service = listenerEventV2Service;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<HealthReportV2> RetrieveHealthReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventV2> events =
                await this.eventV2Service.RetrieveAllEventV2sAsync(cancellationToken);

            IQueryable<ListenerEventV2> listenerEvents =
                await this.listenerEventV2Service.RetrieveAllListenerEventV2sAsync(cancellationToken);

            long totalEvents = events.LongCount();

            long totalActive = events
                .LongCount(@event => @event.Status == EventStatusV2.Active);

            long totalImmediate = events
                .LongCount(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Immediate);

            long totalScheduled = events
                .LongCount(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Scheduled);

            long totalQuarantined = events
                .LongCount(@event => @event.Status == EventStatusV2.Quarantined);

            long totalListenerEvents = listenerEvents.LongCount();

            long totalSuccess = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success);

            long totalError = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error);

            long totalActiveRetries = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = listenerEvents
                .LongCount(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,

                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Events",
                        value: totalEvents,
                        description: "Total number of events created."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Active",
                        value: totalActive,
                        description: "Total number of active events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Immediate",
                        value: totalImmediate,
                        description: "Total number of active immediate events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Scheduled",
                        value: totalScheduled,
                        description: "Total number of active scheduled events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Total Quarantined",
                        value: totalQuarantined,
                        description: "Total number of quarantined events."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Loops Detected",
                        value: totalQuarantined,
                        description: "Number of events quarantined by loop detection."),

                    MapToHealthCheckItem(
                        grouping: "Active Events",
                        item: "Duplicates Blocked",
                        value: totalQuarantined,
                        description: "Number of events blocked as duplicates."),

                    MapToHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Total Listener Events",
                        value: totalListenerEvents,
                        description: "Total number of listener events."),

                    MapToRateHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Total Success",
                        count: totalSuccess,
                        total: totalListenerEvents,
                        description: "Listener events that completed successfully."),

                    MapToRateHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Total Error",
                        count: totalError,
                        total: totalListenerEvents,
                        description: "Listener events that ended in an error state."),

                    MapToHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Active (Retries Left)",
                        value: totalActiveRetries,
                        description: "Errored listener events with retry attempts remaining."),

                    MapToHealthCheckItem(
                        grouping: "Active Listeners",
                        item: "Dead (No Retries)",
                        value: totalDead,
                        description: "Errored listener events with no retry attempts remaining.")
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
