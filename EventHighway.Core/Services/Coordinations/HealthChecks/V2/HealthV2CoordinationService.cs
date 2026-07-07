// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService : IHealthV2CoordinationService
    {
        private readonly IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService;
        private readonly IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService;
        private readonly IHealthArchivedEventsV2OrchestrationService healthArchivedEventsV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public HealthV2CoordinationService(
            IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService,
            IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService,
            IHealthArchivedEventsV2OrchestrationService healthArchivedEventsV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.healthInfrastructureV2OrchestrationService = healthInfrastructureV2OrchestrationService;
            this.healthEventsV2OrchestrationService = healthEventsV2OrchestrationService;
            this.healthArchivedEventsV2OrchestrationService = healthArchivedEventsV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<HealthReportV2> RetrieveHealthCheckItemsReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HealthReportV2 infrastructurePartialReport =
                await this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 eventsPartialReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 archivedEventsPartialReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(period, windowStart, cancellationToken);

            HealthReportV2 report = await BuildReportShellAsync(period, windowStart);
            HealthConfiguration healthConfiguration = this.configurationBroker.GetHealthConfiguration();

            IReadOnlyList<HealthCheckItemV2> healthCheckItems = MergeHealthCheckItems(
                infrastructurePartialReport, eventsPartialReport, archivedEventsPartialReport);

            ScoreHealthCheckItems(healthCheckItems, healthConfiguration);
            report.HealthCheckItems = healthCheckItems;

            return report;
        }

        private async ValueTask<HealthReportV2> BuildReportShellAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart)
        {
            DateTimeOffset windowEnd = ComputeWindowEnd(period, windowStart);
            DateTimeOffset generatedDate = await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                WindowLabel = BuildWindowLabel(period, windowStart, windowEnd),
                GeneratedDate = generatedDate
            };
        }

        private static IReadOnlyList<HealthCheckItemV2> MergeHealthCheckItems(
            params HealthReportV2[] partialReports)
        {
            return partialReports
                .Where(partialReport => partialReport?.HealthCheckItems != null)
                .SelectMany(partialReport => partialReport.HealthCheckItems)
                .ToList();
        }

        private static void ScoreHealthCheckItems(
            IReadOnlyList<HealthCheckItemV2> healthCheckItems,
            HealthConfiguration healthConfiguration)
        {
            foreach (HealthCheckItemV2 healthCheckItem in healthCheckItems)
            {
                HealthMetric? metric = MapToHealthMetric(healthCheckItem.Grouping, healthCheckItem.Item);

                if (metric is null)
                {
                    continue;
                }

                decimal? metricValue = ParseMetricValue(healthCheckItem.Value);

                if (metricValue is null)
                {
                    continue;
                }

                HealthStatusV2 status =
                    ComputeRagStatus(metricValue.Value, metric.Value, healthConfiguration);

                healthCheckItem.StatusCode = (int)status;
                healthCheckItem.Status = status.ToString();
            }
        }

        private static HealthMetric? MapToHealthMetric(string grouping, string item)
        {
            switch (grouping, item)
            {
                case ("Infrastructure", "Registered Handlers"):
                    return HealthMetric.HandlerCount;

                case ("Active Events", "Total Quarantined"):
                case ("Active Events", "Loops Detected"):
                case ("Active Events", "Duplicates Blocked"):
                    return HealthMetric.LoopsDetected;

                case ("Active Listeners", "Total Error"):
                    return HealthMetric.ErrorRate;

                case ("Active Listeners", "Dead (No Retries)"):
                    return HealthMetric.DeadEvents;

                case ("Archived Listeners", "Total Error"):
                    return HealthMetric.ArchiveErrorRate;

                case ("Archived Listeners", "Dead (No Retries)"):
                    return HealthMetric.DeadArchivedEvents;

                default:
                    return null;
            }
        }

        private static decimal? ParseMetricValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int openParenthesisIndex = value.IndexOf('(');
            int percentIndex = value.IndexOf('%');

            string numericPart = openParenthesisIndex >= 0 && percentIndex > openParenthesisIndex
                ? value.Substring(openParenthesisIndex + 1, percentIndex - openParenthesisIndex - 1)
                : value;

            return decimal.TryParse(
                numericPart, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue)
                    ? parsedValue
                    : (decimal?)null;
        }

        private static HealthStatusV2 ComputeRagStatus(
            decimal value,
            HealthMetric metric,
            HealthConfiguration healthConfiguration)
        {
            RagThreshold threshold =
                healthConfiguration?.Thresholds?.FirstOrDefault(ragThreshold => ragThreshold.Metric == metric);

            if (threshold is null)
            {
                return HealthStatusV2.NA;
            }

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

        private static DateTimeOffset ComputeWindowEnd(TrafficPeriodV2 period, DateTimeOffset windowStart)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                    return windowStart.AddDays(7);

                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(windowStart.Year, windowStart.Month, 1, 0, 0, 0, TimeSpan.Zero)
                        .AddMonths(1);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(windowStart.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

                default:
                    return windowStart.AddHours(24);
            }
        }

        private static string BuildWindowLabel(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                    return $"{windowStart.ToString("dd MMM", CultureInfo.InvariantCulture)} – " +
                        $"{windowEnd.AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";

                case TrafficPeriodV2.Month:
                    return windowStart.ToString("MMM yyyy", CultureInfo.InvariantCulture);

                case TrafficPeriodV2.Year:
                    return windowStart.Year.ToString(CultureInfo.InvariantCulture);

                default:
                    return windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            }
        }
    }
}
