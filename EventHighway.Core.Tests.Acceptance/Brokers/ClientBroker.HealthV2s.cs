// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IReadOnlyList<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthStatusClientV2
                .RetrieveHealthRagStatusV2Async(period, windowStart);

        public async ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthTrafficClientV2
                .RetrieveTrafficSnapshotV2Async(period, windowStart);

        public async ValueTask<IReadOnlyList<EventAddressUsageV2>> RetrieveEventAddressSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthAddressClientV2
                .RetrieveEventAddressSummaryV2Async(period, windowStart);

        public async ValueTask<IReadOnlyList<ParticipantUsageV2>> RetrieveParticipantSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthParticipantClientV2
                .RetrieveParticipantSummaryV2Async(period, windowStart);

        public async ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthLoopClientV2
                .RetrieveLoopDetectionSummaryV2Async(period, windowStart);

        public async ValueTask<DuplicateDetectionSummaryV2> RetrieveDuplicateDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthDuplicateClientV2
                .RetrieveDuplicateDetectionSummaryV2Async(period, windowStart);

        public async ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthRetryClientV2
                .RetrieveRetryHealthV2Async(period, windowStart);
    }
}
