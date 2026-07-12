// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<IReadOnlyList<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthStatusClientV2
                    .RetrieveHealthRagStatusV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);

        public ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthTrafficClientV2
                    .RetrieveTrafficSnapshotV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventAddressUsageV2>>
            RetrieveEventAddressSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthAddressClientV2
                    .RetrieveEventAddressSummaryV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);

        public ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthLoopClientV2
                    .RetrieveLoopDetectionSummaryV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);

        public ValueTask<DuplicateDetectionSummaryV2>
            RetrieveDuplicateDetectionSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthDuplicateClientV2
                    .RetrieveDuplicateDetectionSummaryV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);

        public ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthRetryClientV2
                    .RetrieveRetryHealthV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<ParticipantUsageV2>>
            RetrieveParticipantSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthClientV2.HealthParticipantClientV2
                    .RetrieveParticipantSummaryV2Async(period, windowStart, cancellationToken: cancellationToken),
                cancellationToken);
    }
}
