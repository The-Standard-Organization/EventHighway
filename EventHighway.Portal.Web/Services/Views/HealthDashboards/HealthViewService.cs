// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Components.CoreUI;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;

namespace EventHighway.Portal.Web.Services.Views.HealthDashboards
{
    public partial class HealthViewService : IHealthViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public HealthViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<HealthRagTile>> RetrieveHealthRagTilesAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IReadOnlyList<HealthCheckItemV2> healthCheckItems =
                await this.eventHighwayBroker.RetrieveHealthRagStatusV2Async(
                    period, windowStart, cancellationToken: cancellationToken);

            return healthCheckItems.Select(AsRagTile).ToList();
        });

        public ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventHighwayBroker.RetrieveTrafficSnapshotV2Async(
                period, windowStart, cancellationToken: cancellationToken));

        public ValueTask<List<EventAddressUsageV2>> RetrieveAddressSummariesAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IReadOnlyList<EventAddressUsageV2> summaries =
                await this.eventHighwayBroker.RetrieveEventAddressSummaryV2Async(
                    period, windowStart, cancellationToken: cancellationToken);

            return summaries.ToList();
        });

        public ValueTask<LoopDetectionSummaryV2> RetrieveLoopSummaryAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventHighwayBroker.RetrieveLoopDetectionSummaryV2Async(
                period, windowStart, cancellationToken: cancellationToken));

        public ValueTask<DuplicateDetectionSummaryV2> RetrieveDuplicateSummaryAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventHighwayBroker.RetrieveDuplicateDetectionSummaryV2Async(
                period, windowStart, cancellationToken: cancellationToken));

        public ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventHighwayBroker.RetrieveRetryHealthV2Async(
                period, windowStart, cancellationToken: cancellationToken));

        public ValueTask<List<ParticipantUsageV2>> RetrieveParticipantSummariesAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IReadOnlyList<ParticipantUsageV2> summaries =
                await this.eventHighwayBroker.RetrieveParticipantSummaryV2Async(
                    period, windowStart, cancellationToken: cancellationToken);

            return summaries.ToList();
        });

        private static HealthRagTile AsRagTile(HealthCheckItemV2 healthCheckItem) =>
            new HealthRagTile
            {
                Grouping = healthCheckItem.Grouping,
                Label = ShortenLabel(healthCheckItem.Grouping, healthCheckItem.Item),
                Value = healthCheckItem.Value,
                Description = healthCheckItem.Description,
                StatusCode = healthCheckItem.StatusCode,
                Variant = AsVariant(healthCheckItem.Status)
            };

        // The "Event Archives" group header already says these are archived, so drop the
        // redundant "Archived" from each tile label (e.g. "Total Archived Events" -> "Total Events").
        private static string ShortenLabel(string grouping, string item) =>
            grouping == "Event Archives"
                ? item.Replace("Archived ", string.Empty).Replace(" Archived", string.Empty)
                : item;

        private static StatTileVariant AsVariant(string status) =>
            status switch
            {
                "Green" => StatTileVariant.Green,
                "Amber" => StatTileVariant.Amber,
                "Red" => StatTileVariant.Red,
                _ => StatTileVariant.Na
            };
    }
}
